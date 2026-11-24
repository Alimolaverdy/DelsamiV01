namespace NopFarsiIR.Payments.Mellat
{
    public static class Utility
    {
        public static string ErrorCode(string codeType = "", string code = "")
        {
            string result;

            switch (code)
            {
                case "0":
                    result = string.Format("تراکنش با موفقیت انجام شد", new object[0]);
                    return result;
                case "11":
                    return "شماره كارت نامعتبر است";

                case "12":
                    result = string.Format("موجودی کافی نیست", new object[0]);
                    return result;
                case "13":
                    return "رمز نادرست است";
                case "14":
                    return "تعداد دفعات وارد كردن رمز بيش از حد مجاز است";

                case "15":
                    return "كارت نامعتبر است";

                case "16":
                    return "دفعات برداشت وجه بيش از حد مجاز است";

                case "17":
                    return "كاربر از انجام تراكنش منصرف شده است";

                case "18":
                    return "تاريخ انقضاي كارت گذشته است";

                case "19":
                    return "مبلغ برداشت وجه بيش از حد مجاز است";

                case "111":
                    return "صادر كننده كارت نامعتبر است";

                case "112":
                    return "خطاي سوييچ صادر كننده كارت";

                case "113":
                    return "پاسخي از صادر كننده كارت دريافت نشد";

                case "114":
                    return "دارنده كارت مجاز به انجام اين تراكنش نيست";

                case "21":
                    return "ذيرنده نامعتبر است";

                case "23":
                    return "خطاي امنيتي رخ داده است";

                case "24":
                    return "اطلاعات كاربري پذيرنده نامعتبر است";

                case "25":
                    return "مبلغ نامعتبر است";

                case "31":
                    return "پاسخ نامعتبر است";

                case "32":
                    return "فرمت اطلاعات وارد شده صحيح نمي باشد";

                case "33":
                    return "حساب نامعتبر است";

                case "34":
                    return "خطاي سيستمي";

                case "35":
                    return "تاريخ نامعتبر است";

                case "41":
                    return "شماره درخواست تكراري است";

                case "42":
                    return "تراكنش Sale يافت نشد";

                case "43":
                    return "قبلا درخواست Verify داده شده است";

                case "421":
                    return "IP نامعتبر است";

                case "51":
                    return "تراكنش تكراري است";

                case "54":
                    return "راكنش مرجع موجود نيست";

                case "55":
                    return "تراكنش نامعتبر است";

                case "خطا در واريز":
                    return "61";

                case "416":
                    return "خطا در ثبت اطلاعات";

                case "415":
                    return "زمان جلسه كاري به پايان رسيده است";

                case "414":
                    return "سازمان صادر كننده قبض نامعتبر است";

                case "413":
                    return "شناسه پرداخت نادرست است";

                case "412":
                    return "شناسه قبض نادرست است";

                case "49":
                    return "تراكنش Refund يافت نشد";



                default:
                    return "خطاهای متفرقه";

            }

        }
    }
}