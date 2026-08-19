using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Helper
{
    public enum ApiResultStatus
    {
        [Description("沒錯誤")]
        None = 1,

        [Description("輸入/業務資料不合法")]
        Validation = 2,

        [Description("找不到資源")]
        NotFound = 3,

        [Description("資料衝突")]
        Conflict = 4
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public ApiResultStatus apiResultStatus = ApiResultStatus.None;
    }
}

