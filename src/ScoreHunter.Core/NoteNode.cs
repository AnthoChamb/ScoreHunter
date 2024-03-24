using ScoreHunter.Core.Interfaces;

namespace ScoreHunter.Core
{
    public class NoteNode : INoteNode
    {
        public NoteNode(INote value)
        {
            Value = value;
        }

        public INote Value { get; }
        public NoteNode Next { get; set; }
        public NoteNode Previous { get; set; }

        public IChordNode GetChordNode()
        {
            var start = Value.Start;

            var first = this;
            while (first.Previous != null && first.Previous.Value.Start == start)
            {
                first = first.Previous;
            }

            var last = this;
            while (last.Next != null && last.Next.Value.Start == start)
            {
                last = last.Next;
            }

            return new Chord(first, last);
        }

        INoteNode INoteNode.Next => Next;
        INoteNode INoteNode.Previous => Previous;
    }
}
