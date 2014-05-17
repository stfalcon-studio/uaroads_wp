using System;
using System.Windows;

namespace UaRoadsWpApi
{
    public class ApiResponseProcessor
    {
        //private static Dictionary<string, bool> _allErrors;

        static ApiResponseProcessor()
        {
            //InitAllErrors();
            //InitErrorsHandlerRules();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="showMessage"></param>
        /// <param name="getSuccess"></param>
        /// <param name="allowErrorCodeProcessing"></param>
        /// <param name="messageBoxStyle"></param>
        /// <param name="beforeErrorAction">сработает в случае, если возникте ошибка, для которой зарегистрирован обработчик</param>
        /// <returns></returns>
        public static bool Process(ApiResponse response, bool showMessage = true,
            Func<ApiResponse, bool> getSuccess = null, bool allowErrorCodeProcessing = false,
            Style messageBoxStyle = null, Action<object> beforeErrorAction = null)
        {
            var success = getSuccess != null ? getSuccess(response) : response.IsSuccess;

            if (success) return true;

            if (showMessage)
            {
                var message = GetErrorMessageText(response);

                if (!String.IsNullOrEmpty(message))
                {
                    ShowMsgBox(message, messageBoxStyle);
                }
            }
            return false;
        }

        public static void ShowMsgBox(string message, Style messageBoxStyle = null)
        {
            //var mb = new CustomMessageBox();
            //if (messageBoxStyle != null)
            //{
            //    mb.Style = messageBoxStyle;
            //}
            //mb.Title = "Error";
            //mb.Message = message;
            //mb.LeftButtonContent = "Ok";
            //mb.Show();

            MessageBox.Show(message, "AppName", MessageBoxButton.OK);
        }

        public static string GetErrorMessageText(ApiResponse response)
        {
            var message = response.message;

            //if (String.IsNullOrEmpty(response.errorCode))
            //{
            //    response.message = GetErrorMessage(response);
            //    message = GetErrorMessage(response);
            //}



            return message;
        }

        //public static string GetErrorMessage(ApiResponse response)
        //{
        //    //todo add
        //    return "error message";
        //    //var code = response.errorCode;
        //    //return GetErrorMessage(code);
        //}

        public static string GetErrorMessage(string code)
        {
            //todo add
            //var message = ResourceManagerHelper.GetString("ErrorCode_" + code);
            return "message";
        }


        //private static void InitErrorsHandlerRules()
        //{


        //}



        //private static void InitAllErrors()
        //{
        //    _allErrors = new Dictionary<string, bool>();

        //    var classType = typeof(ErrorCodes);

        //    var allFields = classType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        //    var allConstants = allFields.Where(x => x.IsLiteral && !x.IsInitOnly).ToList();

        //    foreach (var constant in allConstants)
        //    {
        //        _allErrors.Add((string)constant.GetRawConstantValue(), false);
        //    }
        //}
    }

    public class ErrorDescriptionContainer
    {
        public ErrorDescriptionContainer()
        {

        }

        public ErrorDescriptionContainer(Action<object> actionOnError, bool canDeveloperIgnoreThisError = true)
        {
            CanDeveloperIgnoreThisError = canDeveloperIgnoreThisError;
            ActionOnError = actionOnError;
        }

        /// <summary>
        /// if critic error - developer cant hide error message
        /// </summary>
        public bool CanDeveloperIgnoreThisError { get; set; }
        public Action<object> ActionOnError { get; set; }
    }
}
