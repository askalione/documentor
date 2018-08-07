# `ObjectBuilder`

```csharp
public static class Elibrary.Builder.ObjectBuilder

```

## Static Methods

- `BuildToXml(BaseRootNode<TNode> node)`

   **Return type:** `XDocument`

   Сгенерировать Xml для объекта

   | Type | Name | Description | 
   | --- | --- | --- | 
   | `BaseRootNode<TNode>` | node | Объект | 



- `BuildToXml(TNode node, String test)`

   **Return type:** `XDocument`

   Another comments

   | Type | Name | Description | 
   | --- | --- | --- | 
   | `TNode` | node | Node | 
   | `String` | test | Test | 



- `SaveToXml(BaseRootNode<TNode> node, String path)`

   **Return type:** `void`

   Сохранить объект в формате Xml по указанному пути. Если файл существует, то он будет перезаписан

   | Type | Name | Description | 
   | --- | --- | --- | 
   | `BaseRootNode<TNode>` | node | Объект | 
   | `String` | path | Путь для сохранения Xml | 



- `BuildToJats(BaseRootNode<TNode> node)`

   **Return type:** `XDocument`

   | Type | Name | Description | 
   | --- | --- | --- | 
   | `BaseRootNode<TNode>` | node |  | 



- `SaveToJats(BaseRootNode<TNode> node, String path)`

   **Return type:** `void`

   | Type | Name | Description | 
   | --- | --- | --- | 
   | `BaseRootNode<TNode>` | node |  | 
   | `String` | path |  | 



- `BuildFromXml(String path)`

   **Return type:** `TNode`

   Сгенерировать объект из xml файла

   | Type | Name | Description | 
   | --- | --- | --- | 
   | `String` | path | Путь для чтения xml | 




