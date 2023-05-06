using SerializIt;

namespace SemanticParser.Serializing;

[Serializer(ESerializers.Yaml)]
[YamlOptions(indentChars: "  ", addPreamble: true)]
[SerializeType(typeof(Model.File))]
[SerializeType(typeof(Model.Container))]
[SerializeType(typeof(Model.Node))]
[SerializeType(typeof(Model.ParsingError))]
[SerializeType(typeof(Model.LocationSpan))]
internal partial class YamlContext
{
}
