using UnityEngine;

namespace Weelco.VRKeyboard {

    public abstract class VRKeyboardData : MonoBehaviour {
        
        public const string ABC = "abc";
        public const string SYM = "sym";
        public const string BACK = "BACK";
        public const string ENTER = "ENTER";
        public const string UP = "UP";
        public const string LOW = "LOW";

        public static readonly string[] allLettersLowercase = new string[]
        {
            "1","2","3","4","5","6","7","8","9","0",
            "q","w","e","r","t","y","u","i","o","p","å",
            SYM,"a","s","d","f","g","h","j","k","l","ø",
            UP,"z","x","c","v","b","n","m","æ",BACK,
            ".com","@"," ",".",ENTER
        };

        public static readonly string[] allLettersUppercase = new string[]
        {
            "1","2","3","4","5","6","7","8","9","0",
            "Q","W","E","R","T","Y","U","I","O","P","Å",
            SYM,"A","S","D","F","G","H","J","K","L","Ø",
            LOW,"Z","X","C","V","B","N","M","Æ",BACK,
            ".com","@"," ",".",ENTER
        };

        public static readonly string[] allSpecials = new string[]
        {
            "1","2","3","4","5","6","7","8","9","0",
            "!","~","#","$","%","^","&","*","(",")","[",
            ABC,"-","_","+","=","\\",";",":","'","\"","]",
            "№","{","}","<",">",",","/","?","|",BACK,
            ".com","@"," ",".",ENTER
        };
    }
}