using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ScoreHunter
{
    public class Candidate : ICandidate
    {
        [Flags]
        private enum ScoringFlags : byte
        {
            None = 0b0000_0000,
            HeroPower = 0b000_0001,
            HeroPowerStreak = 0b0000_0010,
            ChainHeroPower = 0b0000_0100
        }

        private class Builder
        {
            private int _score;
            private int _streak;
            private int _multiplier;
            private int _miss;
            private ScoringFlags _flags;
            private IHeroPower _heroPower;
            private int _heroPowerCount;
            private double _heroPowerEnd;
            private IImmutableList<IActivation> _activations;
            private readonly ScoringOptions _options;

            public Builder(Candidate scoring)
            {
                _score = scoring.Score;
                _streak = scoring.Streak;
                _multiplier = scoring._multiplier;
                _miss = scoring.Miss;
                _flags = scoring._flags;
                _heroPower = scoring._heroPower;
                _heroPowerCount = scoring._heroPowerCount;
                _heroPowerEnd = scoring._heroPowerEnd;
                _activations = scoring._activations;
                _options = scoring._options;
            }

            private int Multiplier => _flags.HasFlag(ScoringFlags.HeroPower) ? _heroPower.Multiplier(_multiplier) : _multiplier;
            private int MaxMultiplier => _flags.HasFlag(ScoringFlags.HeroPower) ? _heroPower.MaxMultiplier(_options.MaxMultiplier) : _options.MaxMultiplier;

            public Builder Activate(NoteEvent note)
            {
                _activations = _activations.Add(new Activation(_heroPower, note, _streak + 1, _flags.HasFlag(ScoringFlags.ChainHeroPower)));
                return Activate(note.Start);
            }

            public Builder Activate(SustainEvent sustain)
            {
                _activations = _activations.Add(new Activation(_heroPower, sustain, _streak, _flags.HasFlag(ScoringFlags.ChainHeroPower)));
                return Activate(sustain.Start);
            }

            private Builder Activate(double start)
            {
                _flags |= ScoringFlags.HeroPower;
                _heroPowerCount--;
                _heroPowerEnd = start + _heroPower.Duration;
                return this;
            }

            public Builder HitNote(NoteEvent note)
            {
                _streak++;

                if (_multiplier < MaxMultiplier && _streak % _options.StreakPerMultiplier == 0)
                {
                    _multiplier++;
                }
                else if (_multiplier > MaxMultiplier)
                {
                    _multiplier = MaxMultiplier;
                }

                if (note.IsHeroPowerStart)
                {
                    _flags |= ScoringFlags.HeroPowerStreak;
                }

                _score += (int)Math.Round(note.Frets.Count * Multiplier * _options.PointsPerNote);

                if (_flags.HasFlag(ScoringFlags.HeroPowerStreak) && note.IsHeroPowerEnd)
                {
                    _heroPowerCount++;
                    _flags &= ~ScoringFlags.HeroPowerStreak;
                }

                _flags &= ~ScoringFlags.ChainHeroPower;
                return this;
            }

            public Builder HoldSustain(SustainEvent sustain)
            {
                _score += (int)Math.Round(sustain.Frets.Count * Multiplier * _options.PointsPerSustain);
                return this;
            }

            public Builder Miss()
            {
                _miss = _streak + 1;
                _streak = 0;
                _multiplier = 1;
                _flags &= ~ScoringFlags.HeroPowerStreak;
                return this;
            }

            public Builder SetHeroPower(IHeroPower heroPower)
            {
                _flags &= ~ScoringFlags.HeroPower;
                _flags &= ~ScoringFlags.HeroPowerStreak;
                _heroPower = heroPower;
                _heroPowerCount = 0;
                return this;
            }

            public Builder EndHeroPower()
            {
                _flags &= ~ScoringFlags.HeroPower;
                _flags |= ScoringFlags.ChainHeroPower;
                _heroPowerEnd = 0;
                return this;
            }

            public Builder ExtendHeroPower(double duration)
            {
                _heroPowerEnd += duration;
                return this;
            }

            public Candidate Build()
            {
                return new Candidate(_score, _streak, _multiplier, _miss, _flags, _heroPower, _heroPowerCount, _heroPowerEnd, _activations, _options);
            }
        }

        private class CacheKey : ICacheKey
        {
            private readonly Candidate _scoring;

            public CacheKey(Candidate scoring)
            {
                _scoring = scoring;
            }

            public bool Equals(ICacheKey other)
            {
                return other is CacheKey cacheKey
                       && _scoring.Miss == cacheKey._scoring.Miss
                       && _scoring._heroPower == cacheKey._scoring._heroPower
                       && _scoring._heroPowerCount == cacheKey._scoring._heroPowerCount
                       && _scoring._flags == cacheKey._scoring._flags
                       && _scoring._heroPowerEnd == cacheKey._scoring._heroPowerEnd;
            }

            public override bool Equals(object obj) => obj is ICacheKey other && Equals(other);

            public override int GetHashCode()
            {
                int hashCode = 120811603;
                hashCode = hashCode * -1521134295 + _scoring.Miss.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<IHeroPower>.Default.GetHashCode(_scoring._heroPower);
                hashCode = hashCode * -1521134295 + _scoring._heroPowerCount.GetHashCode();
                hashCode = hashCode * -1521134295 + _scoring._flags.GetHashCode();
                hashCode = hashCode * -1521134295 + _scoring._heroPowerEnd.GetHashCode();
                return hashCode;
            }
        }

        private readonly IHeroPower _heroPower;
        private readonly ScoringOptions _options;
        private readonly ScoringFlags _flags;

        private readonly int _multiplier = 1;
        private readonly int _heroPowerCount;
        private readonly double _heroPowerEnd;

        private readonly IImmutableList<IActivation> _activations = ImmutableList.Create<IActivation>();

        public Candidate(IHeroPower heroPower, ScoringOptions options)
        {
            _heroPower = heroPower;
            _options = options;
        }

        private Candidate(int score,
                        int streak,
                        int multiplier,
                        int miss,
                        ScoringFlags flags,
                        IHeroPower heroPower,
                        int heroPowerCount,
                        double heroPowerEnd,
                        IImmutableList<IActivation> activations,
                        ScoringOptions options)
        {
            Score = score;
            Streak = streak;
            _multiplier = multiplier;
            Miss = miss;
            _flags = flags;
            _heroPower = heroPower;
            _heroPowerCount = heroPowerCount;
            _heroPowerEnd = heroPowerEnd;
            _activations = activations;
            _options = options;
        }

        public int Score { get; }
        public int Streak { get; }
        public int Multiplier => _flags.HasFlag(ScoringFlags.HeroPower) ? _heroPower.Multiplier(_multiplier) : _multiplier;
        public int MaxMultiplier => _flags.HasFlag(ScoringFlags.HeroPower) ? _heroPower.MaxMultiplier(_options.MaxMultiplier) : _options.MaxMultiplier;
        public int StreakPerMultiplier => _options.StreakPerMultiplier;
        public int Miss { get; } = -1;

        public IEnumerable<IActivation> Activations => _activations;

        public ICandidate Advance(Event @event)
        {
            if (_flags.HasFlag(ScoringFlags.HeroPower) && _heroPowerEnd <= @event.Start)
            {
                return ToBuilder().EndHeroPower().Build();
            }
            return this;
        }

        public ICandidate HitNote(NoteEvent note) => ToBuilder().HitNote(note).Build();

        public ICandidate HoldSustain(SustainEvent sustain) => ToBuilder().HoldSustain(sustain).Build();

        public ICandidate Highway(HighwayEvent highway)
        {
            if (_flags.HasFlag(ScoringFlags.HeroPower))
            {
                return ToBuilder().ExtendHeroPower(highway.End - highway.Start).Build();
            }
            return this;
        }

        public bool TryActivate(NoteEvent note, out ICandidate candidate)
        {
            if (CanActivate() && _heroPower.CanActivate(this, note))
            {
                candidate = ToBuilder().Activate(note).Build();
                return true;
            }
            candidate = this;
            return false;
        }

        public bool TryActivate(SustainEvent sustain, out ICandidate candidate)
        {
            if (CanActivate() && _heroPower.CanActivate(this, sustain))
            {
                candidate = ToBuilder().Activate(sustain).Build();
                return true;
            }
            candidate = this;
            return false;
        }

        private bool CanActivate() => _heroPower != null && !_flags.HasFlag(ScoringFlags.HeroPower) && _heroPowerCount > 0;

        public bool TryMiss(NoteEvent note, out ICandidate candidate)
        {
            if (Miss == -1 && Streak < StreakPerMultiplier && Streak > 0)
            {
                candidate = ToBuilder().Miss().Build();
                return true;
            }
            candidate = this;
            return false;
        }

        public bool TrySetHeroPower(IHeroPower heroPower, out ICandidate candidate)
        {
            if (_heroPower != heroPower)
            {
                candidate = ToBuilder().SetHeroPower(heroPower).Build();
                return true;
            }
            candidate = this;
            return false;
        }

        public ICacheKey GetCacheKey() => new CacheKey(this);

        private Builder ToBuilder() => new Builder(this);
    }
}
