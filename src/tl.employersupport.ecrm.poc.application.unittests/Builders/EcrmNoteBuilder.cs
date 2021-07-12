using System;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmNoteBuilder
    {
        public Note Build() =>
            Build(null,
                Guid.Parse("8b622798-8fe3-4ce7-9545-a61d6b3f453e"),
                Guid.Parse("f5588b64-86a3-4ba2-a85c-c4c841173934"));

        public Note Build(Guid parentAccountId, Guid parentContactId) =>
            Build(null, parentAccountId, parentContactId);

        public Note Build(Guid? noteId, Guid parentAccountId, Guid parentContactId) =>
            new()
            {
                Id = noteId,
                ParentAccountId = parentAccountId,
                //ParentContactId = parentContactId,
                Subject = "Test subject",
                NoteText = "Test not text"
            };
    }
}
