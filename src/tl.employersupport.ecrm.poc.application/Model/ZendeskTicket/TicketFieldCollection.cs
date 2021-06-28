using System.Collections;
using System.Collections.Generic;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.Model.ZendeskTicket
{
    public class TicketFieldCollection : IDictionary<long, TicketField>
    {
        private readonly IDictionary<long, TicketField> _ticketFields;

        public TicketFieldCollection()
        {
            _ticketFields = new Dictionary<long, TicketField>();
        }

        public TicketFieldCollection(IDictionary<long, TicketField> ticketFields)
        {
            _ticketFields = ticketFields;
        }

        public void Add(long id, TicketField ticketField)
        {
            _ticketFields.Add(id, ticketField);
        }

        public bool ContainsKey(long key)
        {
            return _ticketFields.ContainsKey(key);
        }

        public bool Remove(long key)
        {
            return _ticketFields.Remove(key);
        }

        public bool TryGetValue(long key, out TicketField value)
        {
            return _ticketFields.TryGetValue(key, out value);
        }

        public TicketField this[long key]
        {
            get => _ticketFields[key];
            set => _ticketFields[key] = value;
        }

        public ICollection<long> Keys => _ticketFields.Keys;
        public ICollection<TicketField> Values => _ticketFields.Values;


        public IEnumerator<KeyValuePair<long, TicketField>> GetEnumerator()
        {
            return _ticketFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<long, TicketField> item)
        {
            _ticketFields.Add(item);
        }

        public void Clear()
        {
            _ticketFields.Clear();
        }

        public bool Contains(KeyValuePair<long, TicketField> item)
        {
            return _ticketFields.Contains(item);
        }

        public void CopyTo(KeyValuePair<long, TicketField>[] array, int arrayIndex)
        {
            _ticketFields.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<long, TicketField> item)
        {
            return _ticketFields.Remove(item);
        }

        public int Count => _ticketFields.Count;
        public bool IsReadOnly => _ticketFields.IsReadOnly;
    }
}
