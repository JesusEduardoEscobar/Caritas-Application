using Backend.Infraestructure.Extensions;
using Backend.Infraestructure.Objects;

namespace Backend.Infraestructure.Implementations
{
    public sealed class GlobalResponse<T> : IGlobalResponse<T> where T : class
    {
        public GlobalResponse()
        {
            Code = string.Empty;
            Message = string.Empty;
        }

        public bool Ok { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; } = default;

        public int RowsCount { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public static int GetTotalPages(int rows)
        {
            int totalPages = 0;
            try
            {
                totalPages = (int)Math.Ceiling((double)rows / HConstants.PagesSize);
            }
            catch (Exception ex)
            {

                throw new Exception();
            }
            return totalPages;

        }
        public static int GetTotalPages2000(int rows)
        {
            int totalPages = 0;
            try
            {
                totalPages = (int)Math.Ceiling((double)rows / HConstants.PagesSize2000);
            }
            catch (Exception ex)
            {

                throw new Exception();
            }
            return totalPages;

        }


        public static GlobalResponse<T> Success()
        {
            return Success("", 0);
        }

        public static GlobalResponse<T> Success(string message, int rows)
        {
            return new GlobalResponse<T> { Ok = true, RowsCount = rows, Message = message };
        }
        public static GlobalResponse<T> Fault(string message, int rows)
        {
            return new GlobalResponse<T> { Ok = false, RowsCount = rows, Message = message };
        }

        public static GlobalResponse<T> Success(T data, int rows, string message = "", string code = "")
        {
            return new GlobalResponse<T> { Ok = true, RowsCount = rows, Message = message, Code = code, Data = data };
        }
        public static GlobalResponse<T> Fault(T data, int rows, string message = "", string code = "")
        {
            return new GlobalResponse<T> { Ok = false, RowsCount = rows, Message = message, Code = code, Data = data };
        }

        public static GlobalResponse<T> SuccessP(T data, int rows, string message = "", string code = "", int CurrentPage = 1)
        {
            return new GlobalResponse<T> { Ok = true, Message = message, Code = code, Data = data, TotalPages = GetTotalPages2000(rows), CurrentPage = CurrentPage };
        }

        public static GlobalResponse<T> SuccessS(T data, int rows, string message = "", string code = "", int CurrentPage = 1)
        {
            return new GlobalResponse<T> { Ok = true, Message = message, Code = code, Data = data, TotalPages = GetTotalPages(rows), CurrentPage = CurrentPage };
        }

        public static GlobalResponse<object> Fault(string message, string errorCode)
        {
            return new GlobalResponse<object>
            {
                Ok = false,
                RowsCount = 0,
                Message = message,
                Code = errorCode

            };
        }

        public static GlobalResponse<T> Fault(string message = "", string errorCode = "", T? data = default!)
        {
            return new GlobalResponse<T>
            {
                Ok = false,
                RowsCount = 0,
                Message = message,
                Code = errorCode,
                Data = data
            };
        }

    }
}