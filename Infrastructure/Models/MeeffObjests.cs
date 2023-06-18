namespace Meeff.Models;

public class UpdateLocationRequest
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Locale { get; set; }
}

public class UpdateLocationResponse
{
    public string ErrorCode { get; set; }
    public ExploreResponse[] Users { get; set; }
    public bool HasMore { get; set; }
    public int Likes { get; set; }
}

public class ExploreResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Gender { get; set; }
    public string NationalityCode { get; set; }
    public string[] LanguageCodes { get; set; }
    public int[] LanguageLevels { get; set; }
    public string[] PhotoUrls { get; set; }
    public int BirthYear { get; set; }
    public string Color { get; set; }
    public string AppVersion { get; set; }
    public string Platform { get; set; }
    public string Locale { get; set; }
    public bool IsCustomLoc { get; set; }
    public string GmtOffset { get; set; }
    public string RecentAt { get; set; }
    public string DeviceGmtOffset { get; set; }
    public string LocUpdated { get; set; }
    public bool IsFacebookUser { get; set; }
    public bool IsVipUser { get; set; }
    public bool IsScammer { get; set; }
    public double Distance { get; set; }
    public string AirbridgeId { get; set; }
}


public class LoginRequest
{
    public string Platform { get; set; }
    public string PushToken { get; set; }
    public string DeviceInfo { get; set; }
    public string AppVersion { get; set; }
    public string DeviceNationalityCode { get; set; }
    public string DeviceUniqueId { get; set; }
    public string DeviceLocale { get; set; }
    public string DeviceGmtOffset { get; set; }
    public int DeviceRooted { get; set; }
    public int DeviceEmulator { get; set; }
    public string DeviceEmulatorInfo { get; set; }
    public string Locale { get; set; }
}

public class LoginResponse
{
    public string ErrorCode { get; set; }
    public MeeffUser User { get; set; }
    public AnswerInfo AnswerInfo { get; set; }
    public bool IsRequiredToChangePassword { get; set; }
}

public class MeeffUser
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Gender { get; set; }
    public string NationalityCode { get; set; }
    public string[] LanguageCodes { get; set; }
    public int[] LanguageLevels { get; set; }
    public string[] PhotoUrls { get; set; }
    public bool IsDisabledByAdmin { get; set; }
    public int BirthYear { get; set; }
    public int BirthMonth { get; set; }
    public int BirthDay { get; set; }
    public string Color { get; set; }
    public string AppVersion { get; set; }
    public string Platform { get; set; }
    public string DeviceUniqueId { get; set; }
    public string Session { get; set; }
    public string Locale { get; set; }
    public bool IsCustomLoc { get; set; }
    public string GmtOffset { get; set; }
    public bool IsLogged { get; set; }
    public string Created { get; set; }
    public string RecentAt { get; set; }
    public string DevicePlatform { get; set; }
    public string DeviceLocale { get; set; }
    public string DeviceGmtOffset { get; set; }
    public string LocUpdated { get; set; }
    public bool UnpushAll { get; set; }
    public bool UnpushBoth { get; set; }
    public bool UnpushOne { get; set; }
    public bool UnpushChat { get; set; }
    public int Ruby { get; set; }
    public bool IsFacebookUser { get; set; }
    public bool IsAppleUser { get; set; }
    public bool IsGoogleUser { get; set; }
    public string Email { get; set; }
    public int[] FilterBirthYear { get; set; }
    public int FilterDistance { get; set; }
    public int FilterGenderType { get; set; }
    public string[] FilterLanguageCodes { get; set; }
    public string FilterNationalityCode { get; set; }
    public bool FilterNationalityBlock { get; set; }
    public object[] FilterDeniedNationality { get; set; }
    public bool IsPhoneVerified { get; set; }
    public Attendances Attendances { get; set; }
    public bool IsVipUser { get; set; }
}

public class Attendances
{

}

public class AnswerInfo
{
    public object[] Answers { get; set; }
    public bool HasMore { get; set; }
}

public class RegisterGoogleRequest
{
    public string GoogleId { get; set; }
    public string GoogleToken { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public int BirthYear { get; set; }
    public int BirthMonth { get; set; }
    public int BirthDay { get; set; }
    public string NationalityCode { get; set; }
    public int NationalityBlock { get; set; }
    public string Languages { get; set; }
    public string Levels { get; set; }
    public string Description { get; set; }
    public string DeviceInfo { get; set; }
    public string AppVersion { get; set; }
    public string DeviceUniqueId { get; set; }
    public string Photos { get; set; }
    public string Color { get; set; }
    public string Locale { get; set; }
}

public class UploadReqResponse
{
    public string ErrorCode { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public string Host { get; set; }
    public string ContentType { get; set; }
    public string Acl { get; set; }
    public string XAmzMetaUuid { get; set; }
    public string XAmzAlgorithm { get; set; }
    public UploadImageInfoList[] UploadImageInfoList { get; set; }
}

public class UploadImageInfoList
{
    public string UploadImagePath { get; set; }
    public string Key { get; set; }
    public string XAmzCredential { get; set; }
    public string XAmzDate { get; set; }
    public string Policy { get; set; }
    public string XAmzSignature { get; set; }
}

public class CheckGoogleIdEmailRequest
{
    public string GoogleId { get; set; }
    public string GoogleEmail { get; set; }
    public string Locale { get; set; }
}

public class CheckGoogleIdEmailResponse
{
    public string ErrorCode { get; set; }
    public bool GoogleIdExist { get; set; }
    public bool GoogleEmailExist { get; set; }
}
