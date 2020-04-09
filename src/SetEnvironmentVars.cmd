
::this must be run before starting Visual Studio
::using setx will make these env variables available for future sessions
::note there is no = sign here

setx CONTENT_MODERATOR_SUBSCRIPTION_KEY  "PutSubscriptionKeyHere"
setx CONTENT_MODERATOR_ENDPOINT  "PutYourEndpointHere"


::set intg test settings
setx CONTENT_MODERATOR_SUBSCRIPTION_KEY_INTGTEST  "PutSubscriptionKeyHere"
setx CONTENT_MODERATOR_ENDPOINT_INTGTEST  "PutYourEndpointHere"
