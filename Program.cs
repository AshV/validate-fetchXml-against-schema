using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using static System.Console;

namespace ValidateFetchXmlAgainstSchema
{
    class Program
    {
        static void Main(string[] args)
        {
            var validateFetchXml = ProcessFetchXml(@"
                    <fetch mapping='logical'>
                      <entity name='myentity'>
                        <filter type='and'> 
                          <condition attribute='lastname' operator='like' value='%AshV%' /> 
                        </filter> 
                      </entity> 
                    </fetch>");

            WriteLine(
                validateFetchXml.Success ?
                "fetchXml is valid" :
                $"fetchXml is invalid because {validateFetchXml.Message}");
        }

        public static (bool Success, string Message) ProcessFetchXml(string fetchXml)
        {
            try
            {
                // Loading fetchXml schema
                XmlSchema schema = null;
                using (XmlReader reader = XmlReader.Create("fetch.xsd"))
                {
                    schema = XmlSchema.Read(reader, null);
                }

                // Initiating XmlReaderSettings with schema to validate fetchXml
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(schema);

                // Reading and Validating fetchXml
                byte[] byteArray = Encoding.ASCII.GetBytes(fetchXml);
                MemoryStream stream = new MemoryStream(byteArray);
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    // fetchXml is validated on call of Load()
                    xmldoc.Load(reader);

                    // Do further process here or just return sucess to calling function
                    return (true, "");
                }
            }

            catch (Exception ex)
            {
                // Log error message or return to calling function.
                return (false, ex.Message);
            }
        }
    }
}