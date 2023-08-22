namespace WomPlatform.Connector.Models.RegistryResponses {

    internal class CreateApiKeySourceResponse {

        public string SourceId { get; set; }

        public string Selector { get; set; }

        public KindOfApiKey Kind { get; set; }

        public string ApiKey { get; set; }

    }

}
