using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SenseHatApi.Helpers;

namespace RaceGame
{
    public class GameEngine
    {
        private const int SkipTicks = 180;
        private readonly DeviceManager _deviceManager;

        private readonly Random _randomGen = new Random();
        private bool _initialize;
        private int _loopCounter;
        private int _middle;
        private int _nextGameTick;
        private int _roadPosition;
        private List<Color[]> _screenMatrix;
        private bool _shouldCalculateInitialPosition;
        private bool _waitFirstTry;

        public GameEngine(DeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            _initialize = true;
        }

        public async Task Run()
        {
            if (_initialize)
            {
                InitializeGameEngine();
            }

            if (_waitFirstTry)
            {
                await Task.Delay(2000);
                _waitFirstTry = false;
            }

            var currentY = _deviceManager.SensorHat.SensorData.fusionPose.Y;
            var sleepTime = _nextGameTick - Environment.TickCount;

            if (sleepTime <= 0)
            {
                SetupRoadPosition();

                _screenMatrix.Insert(0, CreateRoadLine(_roadPosition));

                DisplayMatrix(currentY);

                _nextGameTick += SkipTicks;
            }
        }

        private void DisplayMatrix(double currentY)
        {
            if (_screenMatrix.Count != 8) return;

            var last = _screenMatrix.Last();

            if (_shouldCalculateInitialPosition)
            {
                CalculateInitialPosition(last);
            }

            FindNextPosition(currentY);

            var previousColor = last[_middle];

            last[_middle] = Helpers.Green;

            var ledMatrix = Helpers.CreateRectangularArray(_screenMatrix);

            _deviceManager.LedHat.WriteLEDMatrix(ledMatrix);


            if (previousColor.R == 255)
            {
                Crash();
                _initialize = true;
            }
            else
            {
                last[_middle] = previousColor;
                _screenMatrix.RemoveAt(7);
            }
        }

        private void FindNextPosition(double currentY)
        {
            if (currentY < -0.2)
            {
                _middle++;
            }
            else if (currentY > 0.2)
            {
                _middle--;
            }

            if (_middle < 0) _middle = 0;
            if (_middle > 7) _middle = 7;
        }

        private void CalculateInitialPosition(Color[] last)
        {
            for (var j = 0; j < 8; j++)
            {
                if (last[j].R == 0 && last[j].G == 0 && last[j].B == 0)
                {
                    _middle = j + 1;
                    break;
                }
            }

            _shouldCalculateInitialPosition = false;
        }

        private void SetupRoadPosition()
        {
            if (_loopCounter++ <= 5) return;

            if (_roadPosition == 6)
            {
                _roadPosition -= 1;
            }
            else if (_roadPosition == 2)
            {
                _roadPosition += 1;
            }
            else
            {
                var positionchange = _randomGen.Next(0, 2);
                if (positionchange == 0)
                {
                    _roadPosition += 1;
                }
                else
                {
                    _roadPosition -= 1;
                }
            }

            _loopCounter = 0;
        }

        private void InitializeGameEngine()
        {
            _loopCounter = 0;
            _roadPosition = 4;
            _screenMatrix = new List<Color[]>();
            _middle = 0;
            _waitFirstTry = true;
            _nextGameTick = Environment.TickCount;
            _shouldCalculateInitialPosition = true;

            _initialize = false;
        }

        private static Color[] CreateRoadLine(int gapPosition)
        {
            var lineArray = new Color[8];

            for (var i = 0; i < 8; i++)
            {
                var colorValue = Helpers.Red;
                if (Math.Abs(i - gapPosition) < 2)
                    colorValue = Helpers.Black;

                lineArray[i] = colorValue;
            }

            return lineArray;
        }

        private async void Crash()
        {
            var crossred = new[,]
            {
                {Helpers.Red, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Red},
                {Helpers.White, Helpers.Red, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Red, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.Red, Helpers.White, Helpers.White, Helpers.Red, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.White, Helpers.Red, Helpers.Red, Helpers.White, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.White, Helpers.Red, Helpers.Red, Helpers.White, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.Red, Helpers.White, Helpers.White, Helpers.Red, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.Red, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Red, Helpers.White},
                {Helpers.Red, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Red}
            };

            var crossblack = new[,]
            {
                {Helpers.Black, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Black},
                {Helpers.White, Helpers.Black, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Black, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.Black, Helpers.White, Helpers.White, Helpers.Black, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.White, Helpers.Black, Helpers.Black, Helpers.White, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.White, Helpers.Black, Helpers.Black, Helpers.White, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.White, Helpers.Black, Helpers.White, Helpers.White, Helpers.Black, Helpers.White, Helpers.White},
                {Helpers.White, Helpers.Black, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Black, Helpers.White},
                {Helpers.Black, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.White, Helpers.Black}
            };

            for (var i = 0; i <= 5; i++)
            {
                _deviceManager.LedHat.WriteLEDMatrix(crossred);
                await Task.Delay(200);
                _deviceManager.LedHat.WriteLEDMatrix(crossblack);
                await Task.Delay(200);
            }

            await Task.Delay(1000);
        }
    }
}