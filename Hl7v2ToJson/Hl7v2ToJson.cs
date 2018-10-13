using HL7.Dotnetcore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DannyBoyNg
{
    public class Hl7v2ToJson
    {
        public static string Convert(string hl7v2message)
        {
            //if no Message Control Id, then insert 0
            var tmp = hl7v2message.Split('|');
            if (string.IsNullOrWhiteSpace(tmp[9]))
            {
                tmp[9] = "0";
                hl7v2message = string.Join("|", tmp);
            }

            //Parse Hl7 message
            var message = new Message(hl7v2message);
            message.ParseMessage();

            //Process all segments and return JSON formatted string
            return JsonConvert.SerializeObject(ProcessSegments(message.Segments()));
        }

        private static Dictionary<string, List<Dictionary<int, dynamic>>> ProcessSegments(List<Segment> segments)
        {
            var level1 = new Dictionary<string, List<Dictionary<int, dynamic>>>();
            foreach (var segment in segments)
            {
                if (!level1.ContainsKey(segment.Name)) level1.Add(segment.Name, new List<Dictionary<int, dynamic>>());
                var level2 = ProcessFields(segment.GetAllFields());
                level1[segment.Name].Add(level2);
            }
            return level1;
        }

        private static List<dynamic> ProcessRepetitions(List<Field> fields)
        {
            var repeats = new List<dynamic>();
            var counter = 1;
            foreach (var field in fields)
            {
                if (field.IsComponentized)
                {
                    repeats.Add(ProcessComponents(field.Components()));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(field.Value)) repeats.Add(field.Value);
                }
                counter++;
            }
            return repeats;
        }

        private static Dictionary<int, dynamic> ProcessFields(List<Field> fields)
        {
            var level2 = new Dictionary<int, dynamic>();
            var counter = 1;
            foreach (var field in fields)
            {
                if (field.IsComponentized)
                {
                    level2.Add(counter, ProcessComponents(field.Components()));
                }
                else if (field.HasRepetitions)
                {
                    level2.Add(counter, ProcessRepetitions(field.Repetitions()));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(field.Value)) level2.Add(counter, field.Value);
                }
                counter++;
            }
            return level2;
        }

        private static Dictionary<int, dynamic> ProcessComponents(List<Component> components)
        {
            var level3 = new Dictionary<int, dynamic>();
            var counter = 1;
            foreach (var component in components)
            {
                if (component.IsSubComponentized)
                {
                    var level4 = ProcessSubComponents(component.SubComponents());
                    level3.Add(counter, level4);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(component.Value)) level3.Add(counter, component.Value);
                }
                counter++;
            }
            return level3;
        }

        private static Dictionary<int, string> ProcessSubComponents(List<SubComponent> subComponents)
        {
            var level4 = new Dictionary<int, string>();
            var counter = 1;
            foreach (var subComponent in subComponents)
            {
                if (!string.IsNullOrWhiteSpace(subComponent.Value)) level4.Add(counter, subComponent.Value);
                counter++;
            }
            return level4;
        }
    }
}
