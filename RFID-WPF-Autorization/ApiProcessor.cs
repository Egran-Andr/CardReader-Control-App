using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RFID_WPF_Autorization
{
    class ApiProcessor
    {
        //get user by id
        public static async Task<UserModel> GetUser(int userid)
        {
            string url = $"/users/user?id={userid}";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    UserModel user = await response.Content.ReadAsAsync<UserModel>();
                    return user;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //create new user
        public static async Task<Uri> CreateUser(UserModel user)
        {
            string url = $"/users/";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsJsonAsync(url, user))
            {
                if (response.IsSuccessStatusCode)
                {
                    return response.Headers.Location;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //update user info
        public static async Task<UserModel> UpdateUser(int userid, UserModel user)
        {
            string url = $"/users/update/{userid}";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.PutAsJsonAsync(url, user))
            {
                if (response.IsSuccessStatusCode)
                {
                    UserModel updateduser = await response.Content.ReadAsAsync<UserModel>();
                    return updateduser;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //cascade delete all user info
        public static async Task<HttpStatusCode> DeleteUser(int userid)
        {
            string url = $"/users/delete/{userid}";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.DeleteAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return response.StatusCode;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //load image connected to user
        public static async Task<byte[]> LoadImage(int userid)
        {
            string url = $"/users/image?id={userid}";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    byte[] photo = await response.Content.ReadAsByteArrayAsync();
                    return photo;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //load new image to server
        public static async Task<HttpStatusCode> UploadImage(string pathFile)
        {
            string url = $"users/upload_image";
            using (ApiHelper.ApiClient)
            {
                var surveyBytes = File.ReadAllBytes(pathFile);

                var byteArrayContent = new ByteArrayContent(surveyBytes);
                byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

                var response = await ApiHelper.ApiClient.PostAsync(ApiHelper.ApiClient.BaseAddress + url, new MultipartFormDataContent
    {
        {byteArrayContent, "\"file\"", $"\"{Path.GetFileName(pathFile)}\""}});

                return response.StatusCode; 
            }
        }

        //get list of all users
        public static async Task<List<FullUser>> GetAllUsers()
        {
            string url = $"/users/user/all";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    List<FullUser> userlist = await response.Content.ReadAsAsync<List<FullUser>>();
                    return userlist;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //create workplace
        public static async Task<Uri> CreateWorkplace(WorkplaceModel workplacename)
        {
            string url = $"/workplace/";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsJsonAsync(url, workplacename))
            {
                if (response.IsSuccessStatusCode)
                {
                    return response.Headers.Location;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //create new history entry
        public static async Task<Uri> NewHistoryEntry(HistoryModel newentry)
        {
            string url = $"/users/historyadd";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsJsonAsync(url, newentry))
            {
                if (response.IsSuccessStatusCode)
                {
                    return response.Headers.Location;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //connect new card to user
        public static async Task<Uri> NewCardConnection(CardConnectionModel newconection)
        {
            string url = $"/users/cardconnect";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsJsonAsync(url, newconection))
            {
                if (response.IsSuccessStatusCode)
                {
                    return response.Headers.Location;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        //get user histoty
        public static async Task<List<HistoryModel>> GetUserHistory(int userid)
        {
            string url = $"/users/gethistory/{userid}";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    List<HistoryModel> historylist = await response.Content.ReadAsAsync<List<HistoryModel>>();
                    return historylist;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //get users all connected cards
        public static async Task<List<CardConnectionModel>> GetUserConnectedCards(int userid=0)
        {
            string url = "";
            if (userid != 0) {  url = $"/users/cardlist/?id={userid}"; }  
            else {  url = $"/users/cardlist/"; }

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    List<CardConnectionModel> Cardlist = await response.Content.ReadAsAsync<List<CardConnectionModel>>();
                    return Cardlist;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //get history by period
        public static async Task<List<HistoryModel>> getPeriodHistory(string datebegin,string dateend,int userid=0)
        {
            string url = "";
            if (userid != 0) { url = $"/users/gethistory/{datebegin}/{dateend}?id={userid}"; }
            else { url = $"/users/gethistory/{datebegin}/{dateend}"; }


            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    List<HistoryModel> periodhistorylist = await response.Content.ReadAsAsync<List<HistoryModel>>();
                    return periodhistorylist;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //delete card by Rfid number
        public static async Task<HttpStatusCode> deleteCardconnection(string rfidid)
        {
            string url = $"/users/deletecard/{rfidid}";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.DeleteAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return response.StatusCode;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
