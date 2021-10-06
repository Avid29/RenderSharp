using System;

namespace RenderSharp.Render.Analyzer
{
    public class RenderAnalyzer
    {
        private DateTime _startTime;
        private DateTime _endTime;

        public TimeSpan RenderTime => _endTime - _startTime;

        public void Begin()
        {
            _startTime = DateTime.Now;
        }

        public void End()
        {
            _endTime = DateTime.Now;
        }
    }
}
