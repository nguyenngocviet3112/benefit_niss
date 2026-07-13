using log4net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TimorINSSBackEnd.DataContracts.RequestDataContract;

namespace TimorINSSBackEnd.DataContracts.ResponseDataContract
{
    [DataContract]
    public class ResponseBaseDataContract
    {
        [DataMember]
        public List<Error> Errors { get; set; } = new List<Error>();

        // Không chặn hành động (khác Errors) — dùng để báo cho người dùng biết 1
        // bút toán kế toán vừa được hệ thống tự sinh (để kiểm tra lại nếu sai),
        // hoặc bị bỏ qua vì thiếu cấu hình tài khoản Nợ/Có (2026-07-13).
        [DataMember]
        public List<string> Warnings { get; set; } = new List<string>();

        [DataMember]
        public string RequestId { get; set; }

        public bool ManageErrors(string call, ILog log, RequestBaseDataContract request)
        {
            //limit documents saved in database when logging to 20 characters
            if (request != null)
            {
                List<PropertyInfo> docClasses = request.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ContainsDocumentAttribute))).ToList();
                object? valueClass;
                PropertyInfo doc;
                string documentString;
                foreach (PropertyInfo docClass in docClasses)
                {
                    valueClass = docClass.GetValue(request);
                    if (valueClass != null)
                    {
                        if (docClass.PropertyType.FullName.StartsWith("System.Collections.Generic.List"))
                        {
                            foreach (var document in (ICollection)valueClass)
                            {
                                doc = document.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(DocumentAttribute))).Single();
                                documentString = doc.GetValue(document)?.ToString();
                                if (documentString != null && documentString.Length > 20)
                                {
                                    documentString = documentString.Substring(0, 20);
                                }
                                doc.SetValue(document, documentString);
                            }
                        }
                        else
                        {
                            doc = valueClass.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(DocumentAttribute))).Single();
                            documentString = doc.GetValue(valueClass)?.ToString();
                            if (documentString != null && documentString?.Length > 20)
                            {
                                documentString = documentString.Substring(0, 20);
                            }
                            doc.SetValue(valueClass, documentString);
                        }
                    }
                }

                docClasses = request.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(DocumentAttribute))).ToList();
                foreach (PropertyInfo docProp in docClasses)
                {
                    documentString = docProp.GetValue(request)?.ToString().Substring(0, 20);
                    docProp.SetValue(request, documentString);
                }
            }

            if (this.Errors.Count > 0)
            {
                String firstLine = String.Format("Erro na chamada ao serviço: '{0}'  com o request: {1}", call, request == null ? null : JsonConvert.SerializeObject(request));
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(firstLine);
                foreach (Error error in this.Errors)
                    sb.AppendLine(String.Format(" => ERROR: {0}", JsonConvert.SerializeObject(error)));

                log.Error(sb);
                return true;
            }
            else
            {
                log.Info(String.Format("Chamada ao serviço: '{0}'  com o request: {1}", call, request == null ? null : JsonConvert.SerializeObject(request)));
                return false;
            }
        }
    }

    [DataContract]
    public class Error
    {
        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}