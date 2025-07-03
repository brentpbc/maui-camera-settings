namespace MauiCameraSettings;

public class Constants
{
    public static int MAX_ALERT_MSG_LENGTH = 150;
    
    public static class DialogActions
    {
        public const string EMAIL_ACTION = "Email";
        public const string CANCEL_ACTION = "Cancel";
        public const string COPY_ACTION = "Copy to Clipboard";
        public const string SHARE_ACTION = "Share";
    }
    
    public static class Photos
    {
        public static string DEFAULT_PHOTO_FILE_EXTENSTION = "JPG";
        public static int MAX_MULTI_PHOTO_COUNT = 10;
        
        public const string PHOTO_SOURCE_CAMERA = "Camera";
        public const string PHOTO_SOURCE_LIBRARY = "Photo Library";
    }
    public static class Camera
    {
        public static bool SAVE_PHOTO_META_DATA = true;
        public static bool SAVE_PHOTOS_TO_ALBUM = false;
        public static bool POST_PROCESS_PHOTO = true;
        public static int MAX_COMPRESSION_QLTY = 100;
        //iOS and other platforms
        public static int SAVE_PHOTO_COMPRESSION_QLTY = 100;
        public static int SAVE_PHOTO_SIZE_PCT = 70;
        public static bool RESTORE_PHOTO_EXIF = false;
        //Android 
        public static int SAVE_PHOTO_COMPRESSION_QLTY_AND = 25;
        public static int SAVE_PHOTO_SIZE_PCT_AND = 100;
        public static bool RESTORE_PHOTO_EXIF_AND = true;
    }
}