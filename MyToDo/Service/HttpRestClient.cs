using MyToDo.Shared;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;

        public HttpRestClient(string apiUrl)
        {
            this.apiUrl = apiUrl;
            this.client = new RestClient(baseUrl: apiUrl);
        }

        public async Task<ApiResponse> ExecuteAsync(BaseRequest baseRequest)
        {
            var request = new RestRequest(baseRequest.Route, baseRequest.Method);

            if (baseRequest.Parameter != null)
                request.AddParameter("param", JsonConvert.SerializeObject(baseRequest.Parameter), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            // 空值检查和错误处理
            if (string.IsNullOrEmpty(response.Content))
            {
                return new ApiResponse($"API返回空内容. 状态码: {response.StatusCode}");
            }

            try
            {
                return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            }
            catch (JsonException ex)
            {
                return new ApiResponse($"反序列化失败: {ex.Message}");
            }
        }

        public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
        {
            var request = new RestRequest(baseRequest.Route, baseRequest.Method);

            if (baseRequest.Parameter != null)
                request.AddJsonBody(baseRequest.Parameter);

            var response = await client.ExecuteAsync(request);

            // 空值检查和错误处理
            if (string.IsNullOrEmpty(response.Content))
            {
                return new ApiResponse<T>($"API返回空内容. 状态码: {response.StatusCode}");
            }

            try
            {
                return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);
            }
            catch (JsonException ex)
            {
                return new ApiResponse<T>($"反序列化失败: {ex.Message}");
            }
        }
    }
}
