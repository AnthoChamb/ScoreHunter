using ScoreHunter.Core.Enums;

namespace ScoreHunter.Core
{
    public readonly struct Frets
    {
        private Frets(FretFlags flags, int count)
        {
            Flags = flags;
            Count = count;
        }

        public FretFlags Flags { get; }
        public int Count { get; }

        public Frets Add(Frets frets)
        {
            return new Frets(Flags | frets.Flags, Count + frets.Count);
        }

        public Frets Remove(Frets frets)
        {
            return new Frets(Flags & (~frets.Flags), Count - frets.Count);
        }

        public override bool Equals(object obj) => obj is Frets frets && Flags == frets.Flags;
        public override int GetHashCode() => Flags.GetHashCode();

        public static readonly Frets Open = new Frets(FretFlags.Open, 1);
        public static readonly Frets Black1 = new Frets(FretFlags.Black1, 1);
        public static readonly Frets Black2 = new Frets(FretFlags.Black2, 1);
        public static readonly Frets Black3 = new Frets(FretFlags.Black3, 1);
        public static readonly Frets White1 = new Frets(FretFlags.White1, 1);
        public static readonly Frets White2 = new Frets(FretFlags.White2, 1);
        public static readonly Frets White3 = new Frets(FretFlags.White3, 1);

        public static readonly Frets Barre1 = new Frets(FretFlags.Black1 | FretFlags.White1, 2);
        public static readonly Frets Barre2 = new Frets(FretFlags.Black2 | FretFlags.White2, 2);
        public static readonly Frets Barre3 = new Frets(FretFlags.Black3 | FretFlags.White3, 2);
    }
}
