using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Eve.Core.WPA81.UI;
using UR.Core.WP81.API.ApiResponses;

namespace UR.Core.WP81.API
{
    public class ApiResponseProcessor
    {
        //private static Dictionary<string, bool> _allErrors;

        private static List<string> _messageBoxList;

        static ApiResponseProcessor()
        {
            //InitAllErrors();
            //InitErrorsHandlerRules();

            _messageBoxList = new List<string>(3);
        }



        /// <summary>
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

            //if (response.logUserOut)
            //{
            //    //todo не стирать, вомзожно, как временная мера. Сделано затем, что бы данный тип ошибок обрабатывать одинаково. Текст ошибки будет выводится после перехода на окно логина 
            //    //response.errorCode = ErrorCodes.CRITICAL_ERROR;

            //    //AppViewModel.WipeUserData();
            //}

            if (success) return true;




            //var service = Bootstrapper.Current.Container.Resolve<ApiErrorHandlerService>();

            //if (!showMessage)
            //{
            //    if (!service.CanDeveloperIgnoreThis(response.errorCode))
            //    {
            //        showMessage = true;
            //    }
            //}

            //if (showMessage)
            //{
            //    if (service.HasHandler(response.errorCode))
            //    {
            //        showMessage = service.ShowMessageIfCanProcessThisError(response.errorCode);
            //    }
            //}

            ////service.CallErrorHandler(response);

            //if (showMessage)
            //{
            ShowErrorFromResponse(response, messageBoxStyle);
            //}
            return false;
        }

        private static async void ShowErrorFromResponse(ApiResponse response, Style messageBoxStyle)
        {
            if (response.ErrorCode == "NETWORK_ERROR")
            {
                _messageBoxList.Add("Нет подключения к сети, попробуйте позже");
            }
            else
            {
                var errorMessage = response.ErrorMessage;
                //if (response.Errors != null)
                //{
                //    errorMessage += "\r\n";

                //    foreach (var error in response.Errors)
                //    {
                //        errorMessage += error.ErrorMessage + "\r\n";
                //    }
                //}

                errorMessage = errorMessage.Trim();

                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "Произошла неизвестная ошибка, попробуйте позже";
                }

                _messageBoxList.Add(errorMessage);
            }

            Show();
        }

        private static bool isBusy = false;

        private static async void Show()
        {
            if (isBusy) return;

            isBusy = true;
            try
            {
                while (_messageBoxList.Any())
                {
                    var body = _messageBoxList[0];

                    _messageBoxList.RemoveAt(0);

                    if (string.IsNullOrEmpty(body)) continue;

                    await MessageDialogExt.CreateFacade(body, "MEGOGO").WithCommand("Закрыть").ShowAsync();
                }
            }
            finally
            {
                isBusy = false;
                if (_messageBoxList.Any())
                {
                    Show();
                }
            }
        }

    }
}
