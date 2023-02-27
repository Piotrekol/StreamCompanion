using System;

namespace OsuSongsFolderWatcher
{
    public class LazerNullReferenceException : NullReferenceException
    {
        public LazerNullReferenceException(string message) : base(message)
        {
        }
    }
}