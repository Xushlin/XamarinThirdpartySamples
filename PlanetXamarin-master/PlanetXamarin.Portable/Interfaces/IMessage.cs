﻿namespace PlanetXamarin.Portable.Interfaces
{
  public interface IMessage
  {
    void SendMessage(string message, string title = null);
  }
}
