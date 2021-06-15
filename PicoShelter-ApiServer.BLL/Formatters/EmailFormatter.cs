using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;
using System.IO;
using System.Reflection;

namespace PicoShelter_ApiServer.BLL.Formatters
{
    public class EmailFormatter<T> where T : EmailMessageDto
    {
        public EmailFormatter(string templateFilename)
        {
            this.TemplateFilename = templateFilename;
        }

        protected string TemplateFilename { get; set; }
        private static object _templateFileLocker { get; set; } = new object();

        private static string _templateText { get; set; }
        public string TemplateText
        {
            get => _templateText ??= ReadTemplate(TemplateFilename);
        }

        public virtual string Format(T instance)
        {
            return Format(TemplateText, instance);
        }

        protected virtual string Format(string text, T instance)
        {
            var formattedText = text;

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                formattedText = formattedText?.Replace('%' + prop.Name + '%', prop.GetValue(instance)?.ToString(), System.StringComparison.OrdinalIgnoreCase);
            }

            return formattedText;
        }

        protected virtual string ReadTemplate(string templateFilename)
        {
            lock (_templateFileLocker)
            {
                try
                {
                    return File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\EmailTemplates\\" + templateFilename));
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
