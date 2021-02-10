using System;
using Microsoft.Xna.Framework;
using Penumbra;

namespace Nova.Services
{
    public class TimeService : Nova.Common.IUpdateable
    {
        private readonly GameServiceContainer _services;
        private PenumbraComponent _penumbra;

        public DateTime CurrentTime { get; private set; }

        public double OneMinutePassesEveryXMilliseconds { get; set; } = 1000;

        private double _accumulator;

        public TimeService(GameServiceContainer services)
        {
            _services = services;
            _penumbra = _services.GetService<PenumbraComponent>();

            CurrentTime = new DateTime();
            CurrentTime = CurrentTime.AddHours(12);
        }
        
        public void Update(GameTime gameTime)
        {
            _accumulator += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_accumulator > OneMinutePassesEveryXMilliseconds)
            {
                CurrentTime = CurrentTime.AddMinutes(1);
                
                if (CurrentTime.Hour >= 19)
                {
                    var minuteChange = 255d / (5d * 60d);
                    
                    int minute = CurrentTime.Minute + (CurrentTime.Hour - 19) * 60;


                    int calculatedAmbientColor = 255 - (int)(minute * minuteChange);
                    _penumbra.AmbientColor = Color.FromNonPremultiplied(calculatedAmbientColor, calculatedAmbientColor, calculatedAmbientColor, calculatedAmbientColor);
                }
                else if (CurrentTime.Hour >= 4)
                {
                    var minuteChange = 255d / (2d * 60d);
                    
                    int minute = CurrentTime.Minute + (CurrentTime.Hour - 4) * 60;


                    int calculatedAmbientColor = (int)(minute * minuteChange);
                    if (calculatedAmbientColor >= 255)
                        calculatedAmbientColor = 255;

                    _penumbra.AmbientColor = Color.FromNonPremultiplied(calculatedAmbientColor, calculatedAmbientColor, calculatedAmbientColor, calculatedAmbientColor);
                }
                
                _accumulator = 0;
            }
        }
    }
}
