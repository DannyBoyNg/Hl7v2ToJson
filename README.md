# Hl7v2ToJson

A C# library to convert a HL7v2 formatted string to JSON.

## Dependancies

Newtonsoft.Json (>= 11.0.2)  
HL7-dotnetcore (>= 2.5.0)

## Installing

Install from Nuget
```
Install-Package DannyBoyNg.Hl7v2ToJson
```

## Usage

Basic

```csharp
using DannyBoyNg;
...
var jsonFormattedString = Hl7v2ToJson.Convert(hl7v2message);

```

## License

This project is licensed under the MIT License.

## Contributions

Contributions are welcome.