using System.Drawing.Text;

namespace TaycanLogger
{
  public static class FormControlGlobals
  {
    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

    public static Color ColorPower = Color.FromArgb(0, 176, 244);
    public static Color ColorRecup = Color.FromArgb(16, 185, 0);
    public static float TextMarginWidth = 6f;
    public static float TextMarginHeight = 2f;
    public static TextFormatFlags DefaultTextFormatFlags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
    private static PrivateFontCollection m_PrivateFontCollection;
    public static FontFamily FontFamily;
    public static Font FontDisplayTitle;
    public static Font FontDisplayText;

    public static void LoadFonts()
    {
      m_PrivateFontCollection = new PrivateFontCollection();
      byte[] fontData = ResourceFont.PorscheNextAutoTT_Regular;
      IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
      System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
      uint dummy = 0;
      m_PrivateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
      AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
      System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
      FontFamily = m_PrivateFontCollection.Families[0];
      FontDisplayTitle = new Font(FontFamily, 15F, FontStyle.Regular);
      FontDisplayText = new Font(FontFamily, 19F, FontStyle.Regular);
    }

    public static Rectangle ToRectangle(this RectangleF p_RectangleF)
    {
      return new Rectangle((int)p_RectangleF.X, (int)p_RectangleF.Y, (int)p_RectangleF.Width, (int)p_RectangleF.Height);
    }
  }
}