namespace DeltaEngine.Multimedia.VideoStreams.Vlc
{
	internal enum MediaState
	{
		NothingSpecial,
		Opening,
		Buffering,
		Playing,
		Paused,
		Stopped,
		Ended,
		Error
	}
}