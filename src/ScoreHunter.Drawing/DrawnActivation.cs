using ScoreHunter.Core.Events;
using ScoreHunter.Core.Interfaces;
using ScoreHunter.Drawing.Abstractions.Interfaces;

namespace ScoreHunter.Drawing
{
    public class DrawnActivation : IDrawnActivation
    {
        private readonly IActivation _activation;

        public DrawnActivation(IActivation activation, int startTicks)
        {
            _activation = activation;
            StartTicks = startTicks;
            EndTicks = -1;
        }

        public DrawnActivation(DrawnActivation activation, int startTicks)
        {
            _activation = activation._activation;
            StartTicks = startTicks;
            EndTicks = activation.EndTicks;
        }

        public int StartTicks { get; }
        public int EndTicks { get; set; }
        public double Start => _activation.Start;
        public double End => _activation.End;
        public IHeroPower HeroPower => _activation.HeroPower;
        public Event Event => _activation.Event;
        public int Streak => _activation.Streak;
        public bool IsChained => _activation.IsChained;
    }
}
