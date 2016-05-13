using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Http;
using Twilio.Readers;
using Twilio.Resources;
using Twilio.Resources.Api.V2010.Account.Call;

namespace Twilio.Readers.Api.V2010.Account.Call {

    public class NotificationReader : Reader<NotificationResource> {
        private string accountSid;
        private string callSid;
        private int? log;
        private string messageDate;
    
        /**
         * Construct a new NotificationReader
         * 
         * @param accountSid The account_sid
         * @param callSid The call_sid
         */
        public NotificationReader(string accountSid, string callSid) {
            this.accountSid = accountSid;
            this.callSid = callSid;
        }
    
        /**
         * The log
         * 
         * @param log The log
         * @return this
         */
        public NotificationReader byLog(int? log) {
            this.log = log;
            return this;
        }
    
        /**
         * The message_date
         * 
         * @param messageDate The message_date
         * @return this
         */
        public NotificationReader byMessageDate(string messageDate) {
            this.messageDate = messageDate;
            return this;
        }
    
        /**
         * Make the request to the Twilio API to perform the read
         * 
         * @param client ITwilioRestClient with which to make the request
         * @return NotificationResource ResourceSet
         */
        public override async Task<ResourceSet<NotificationResource>> ExecuteAsync(ITwilioRestClient client) {
            Request request = new Request(
                System.Net.Http.HttpMethod.Get,
                Domains.API,
                "/2010-04-01/Accounts/" + this.accountSid + "/Calls/" + this.callSid + "/Notifications.json"
            );
            
            AddQueryParams(request);
            
            Page<NotificationResource> page = await PageForRequest(client, request);
            
            return new ResourceSet<NotificationResource>(this, client, page);
        }
    
        /**
         * Retrieve the next page from the Twilio API
         * 
         * @param nextPageUri URI from which to retrieve the next page
         * @param client ITwilioRestClient with which to make the request
         * @return Next Page
         */
        public override Page<NotificationResource> NextPage(string nextPageUri, ITwilioRestClient client) {
            Request request = new Request(
                System.Net.Http.HttpMethod.Get,
                nextPageUri
            );
            
            var task = PageForRequest(client, request);
            task.Wait();
            
            return task.Result;
        }
    
        /**
         * Generate a Page of NotificationResource Resources for a given request
         * 
         * @param client ITwilioRestClient with which to make the request
         * @param request Request to generate a page for
         * @return Page for the Request
         */
        protected async Task<Page<NotificationResource>> PageForRequest(ITwilioRestClient client, Request request) {
            Response response = await client.Request(request);
            
            if (response == null) {
                throw new ApiConnectionException("NotificationResource read failed: Unable to connect to server");
            } else if (response.GetStatusCode() != HttpStatus.HTTP_STATUS_CODE_OK) {
                RestException restException = RestException.FromJson(response.GetContent());
                if (restException == null)
                    throw new ApiException("Server Error, no content");
                throw new ApiException(
                    restException.GetMessage(),
                    restException.GetCode(),
                    restException.GetMoreInfo(),
                    restException.GetStatus(),
                    null
                );
            }
            
            Page<NotificationResource> result = new Page<NotificationResource>();
            result.deserialize("notifications", response.GetContent());
            
            return result;
        }
    
        /**
         * Add the requested query string arguments to the Request
         * 
         * @param request Request to add query string arguments to
         */
        private void AddQueryParams(Request request) {
            if (log != null) {
                request.AddQueryParam("Log", log.ToString());
            }
            
            if (messageDate != null) {
                request.AddQueryParam("MessageDate", messageDate);
            }
            
            request.AddQueryParam("PageSize", GetPageSize().ToString());
        }
    }
}