using Agora.Rtc;

public class VideoSurfaceClassroom : VideoSurface
{
    public void DestroyTexture()
    {
        Enable = false;
    }

    public VIDEO_SOURCE_TYPE GetVideoSurfaceType()
    {
        return SourceType;
    }
}
