using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DSlide
{
    public class DataContainerFactory
    {
        public HashSet<Assembly> processedAssemblies = new HashSet<Assembly>();
        public int AssemblyCount = 1;

        private Dictionary<Type, Func<DataSlideBase>> constructors = new Dictionary<Type, Func<DataSlideBase>>();

        public T CreateDataContainerInstance<T>() where T : DataSlideBase
        {
            return (T)this.CreateDataContainerInstance(typeof(T));
        }


        public DataSlideBase CreateDataContainerInstance(Type type)
        {
            if (!constructors.ContainsKey(type))
                this.CreateDerivedClasses();

            if (!constructors.ContainsKey(type))
                return null;

            return constructors[type]();
        }

        public void CreateDerivedClasses()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                    .Where(x => !processedAssemblies.Contains(x))
                                    .ToList();

            var derivedTypesNameMapping = new Dictionary<Type, string>();
            var code = this.GenerateCodeToCompile(allAssemblies, derivedTypesNameMapping);


            var syntaxTree = CSharpSyntaxTree.ParseText(code);


            MetadataReference[] references = AppDomain.CurrentDomain.GetAssemblies()
                                                .Where(x => !x.IsDynamic && x.Location != null && x.Location != "")
                                                .Select(x => MetadataReference.CreateFromFile(x.Location))
                                                .ToArray();

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);


            var compilationResult = CSharpCompilation.Create("DSlideGeneratedDerivedClasses" + this.AssemblyCount, new SyntaxTree[] { syntaxTree }, references, options);
            this.AssemblyCount++;

            Assembly compiledAssembly;

            using (var memoryStream = new MemoryStream())
            {
                var emitResult = compilationResult.Emit(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);
                compiledAssembly = Assembly.Load(memoryStream.ToArray());
            }

            foreach (var typeThatIsDerived in derivedTypesNameMapping.Keys)
            {
                var derivedTypeName = derivedTypesNameMapping[typeThatIsDerived];
                var myClassCompiledType = compiledAssembly.GetType(derivedTypeName);
                var myExecuteCompiledMethod = myClassCompiledType.GetMethod("getConstructor");
                var constructorLambda = (Func<DataSlideBase>)myExecuteCompiledMethod.Invoke(null, new object[] { });
                this.constructors[typeThatIsDerived] = constructorLambda;
            }
        }

        public string GenerateCodeToCompile(List<Assembly> assembliesToProcess, Dictionary<Type, string> derivedTypesNameMapping)
        {
            var allTypes = assembliesToProcess.SelectMany(x => x.GetTypes());
            var typesToProcess = allTypes.Where(type => type != typeof(DataSlideBase) &&
                                                        typeof(DataSlideBase).IsAssignableFrom(type));
            var allDerivedClassesCode = "";

            foreach(var type in typesToProcess)
            {
                allDerivedClassesCode += generateDerivedClassCode(type, derivedTypesNameMapping);
            }


            return
$@"
using DSlide;
using System;

{allDerivedClassesCode}
";
        }

        private string generateDerivedClassCode(Type type, Dictionary<Type, string> derivedTypesNameMapping)
        {
            var classContent = "";
            var derivedClassName = type.Name + "DSlideGenerated";
            var baseClassName = type.FullName;
            var dslideBaseClassName = typeof(DSlide.DataSlideBase).FullName;

            derivedTypesNameMapping[type] = "DSlideGenerated." + derivedClassName;

            foreach (var property in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                var getMethod = property.GetGetMethod();
                var setMethod = property.GetSetMethod();

                if (getMethod != null && setMethod != null)
                {
                    if (getMethod.IsAbstract && setMethod.IsAbstract &&
                        !getMethod.IsPrivate && !getMethod.IsFamilyAndAssembly && 
                        !setMethod.IsPrivate && !setMethod.IsFamilyAndAssembly)
                    {
                        classContent += this.generateSourceProperty(property);
                    }
                }
                else if (getMethod != null && setMethod == null)
                {
                    if (getMethod.IsVirtual && !getMethod.IsAbstract &&
                        !getMethod.IsPrivate && !getMethod.IsFamilyAndAssembly)
                    {
                        classContent += this.generateComputedProperty(property);
                    }
                }
            }

            return
$@"
namespace DSlideGenerated
{{
    public sealed class {derivedClassName} : {baseClassName}
    {{
{classContent}

        public static Func<{dslideBaseClassName}> getConstructor()
        {{
            return () => new {derivedClassName}();
        }}
    }}
}}
";
        }

        private string generateSourceProperty(PropertyInfo property)
        {
            var propertyName = property.Name;
            var propertyType = property.PropertyType.FullName;

            return
    $@"
        public override {propertyType} {propertyName}
        {{
            get
            {{
                return base.GetSourceValue<{propertyType}> ();
            }}

            set
            {{
                base.SetSourceValue<{propertyType}> (value);
            }}
        }}
";
        }

        private string generateComputedProperty(PropertyInfo property)
        {
            var propertyName = property.Name;
            var propertyType = property.PropertyType.FullName;

            return
$@"
        public override {propertyType} {propertyName} {{ get {{ return base.GetComputedValue<{propertyType}>(() => base.{propertyName}); }} }}
";
        }
    }
}
