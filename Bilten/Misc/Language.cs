using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Bilten
{
	public class Language
	{
		[DllImport("user32.dll")]
		static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);
		[DllImport("user32.dll")]
		static extern int ActivateKeyboardLayout(IntPtr nkl, uint Flags);
		[DllImport("user32.dll")]
		static extern bool UnloadKeyboardLayout(IntPtr hkl);
		[DllImport("user32.dll")]
		static extern bool GetKeyboardLayoutName([Out] StringBuilder pwszKLID);
		[DllImport("user32")]
		public static extern bool GetKeyboardLayoutName(IntPtr pwszKLID);

		private static String HKL_ENGLISH_US = "00000409";
		private static String HKL_ENGLISH_UK = "00000809";
		private static String HKL_CROATIAN = "0000041A";
		private static String HKL_SERBIAN_CYRILIC = "00000C1A";
		private static String HKL_SERBIAN_LATIN = "0000081A";

		public enum acKeyboardLanguage
		{
			hklEnglishUS = 1,
			hklEnhlishUK = 2,
			hklCroatian = 3,
			hklSerbianCyrilic = 4,
			hklSerbianLatin = 5
		}

		public static Boolean SetKeyboardLanguage(acKeyboardLanguage KeyboardLanguage)
		{
			IntPtr hkl = IntPtr.Zero;
			switch(KeyboardLanguage) 
			{
				case acKeyboardLanguage.hklEnglishUS:      hkl = LoadKeyboardLayout(HKL_ENGLISH_US     , 0); break;
				case acKeyboardLanguage.hklEnhlishUK:      hkl = LoadKeyboardLayout(HKL_ENGLISH_UK     , 0); break;
				case acKeyboardLanguage.hklCroatian:       hkl = LoadKeyboardLayout(HKL_CROATIAN       , 0); break;
				case acKeyboardLanguage.hklSerbianCyrilic: hkl = LoadKeyboardLayout(HKL_SERBIAN_CYRILIC, 0); break;
				case acKeyboardLanguage.hklSerbianLatin:   hkl = LoadKeyboardLayout(HKL_SERBIAN_LATIN  , 0); break;
			}
			if(hkl == IntPtr.Zero) return false;
			return (ActivateKeyboardLayout(hkl, 0) != 0);
		}

		public static Boolean SetKeyboardLanguage(int KeyboardLanguage)
		{
			return SetKeyboardLanguage((acKeyboardLanguage)KeyboardLanguage);
		}

	}

}