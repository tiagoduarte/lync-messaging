using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using System;
using System.Collections.Generic;

namespace LyncMessaging
{
    class LyncManager
    {
        private string _uri;
        private string _message;
        private LyncClient _client;
        private Conversation _conversation;

        private bool _done = false;
        public bool Done
        {
            get { return _done; }
        }

        public LyncManager(string arg0, string arg1)
        {
            _uri = arg0;
            _message = arg1;
            _client = Microsoft.Lync.Model.LyncClient.GetClient();

            //_client.BeginSignIn("email", "domainanduser", "password", BeginSearchCallback, new object[] { _client.ContactManager, _uri });

            _client.ContactManager.BeginSearch(
                _uri,
                SearchProviders.GlobalAddressList,
                SearchFields.EmailAddresses,
                SearchOptions.ContactsOnly,
                2,
                BeginSearchCallback,
                new object[] { _client.ContactManager, _uri }
            );
        }

        private void BeginSearchCallback(IAsyncResult r)
        {
            object[] asyncState = (object[])r.AsyncState;
            ContactManager cm = (ContactManager)asyncState[0];
            try
            {
                SearchResults results = cm.EndSearch(r);
                if (results.AllResults.Count == 0)
                {
                    Console.WriteLine("No results.");
                }
                else if (results.AllResults.Count == 1)
                {
                    //get destination user and communication channel
                    ContactSubscription srs = cm.CreateSubscription();
                    Contact contact = results.Contacts[0];
                    ContactInformationType[] contactInformationTypes = { ContactInformationType.Availability, ContactInformationType.ActivityId };

                    //add user to conversation
                    srs.AddContact(contact);                    
                    srs.Subscribe(ContactSubscriptionRefreshRate.High, contactInformationTypes);
                    _conversation = _client.ConversationManager.AddConversation();
                    _conversation.AddParticipant(contact);

                    //define message to send
                    Dictionary<InstantMessageContentType, String> messages = new Dictionary<InstantMessageContentType, String>();
                    messages.Add(InstantMessageContentType.PlainText, _message);

                    //instantiate conversation
                    InstantMessageModality m = (InstantMessageModality)_conversation.Modalities[ModalityTypes.InstantMessage];
                    m.BeginSendMessage(messages, BeginSendMessageCallback, messages);

                }
                else
                {
                    Console.WriteLine("More than one result.");
                }
            }
            catch (SearchException se)
            {
                Console.WriteLine("Search failed: " + se.Reason.ToString());
            }
            _client.ContactManager.EndSearch(r);
        }

        private void BeginSendMessageCallback(IAsyncResult r)
        {
            //_conversation.End();
            _done = true;
        }
    }
}