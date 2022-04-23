namespace TaycanLogger
{
  internal class FormPages
  {
    private FormPage[] m_FormPages = new FormPage[3];
    private int m_FormPageIndex = 0;
    private Form1 m_FormMain;
    private TableLayoutPanel m_TableLayoutPanel;

    internal IEnumerable<FormPage> Pages { get => m_FormPages; }

    public FormPages(Form1 p_FormMain, TableLayoutPanel p_TableLayoutPanel)
    {
      m_FormMain = p_FormMain;
      m_TableLayoutPanel = p_TableLayoutPanel;
      m_FormPages[0] = new FormPageSettings(m_TableLayoutPanel.ColumnCount);
      m_FormPages[1] = new FormPagePower(m_TableLayoutPanel.ColumnCount);
      m_FormPages[2] = new FormPageTinker(m_TableLayoutPanel.ColumnCount);
      m_FormPages[0].ActivateRequested += FormPage_ActivateRequested;
      m_FormPages[1].ActivateRequested += FormPage_ActivateRequested;
      m_FormPages[2].ActivateRequested += FormPage_ActivateRequested;
      m_TableLayoutPanel.Controls.Add(m_FormPages[0], 0, 0);
      m_TableLayoutPanel.Controls.Add(m_FormPages[1], 0, 0);
      m_TableLayoutPanel.Controls.Add(m_FormPages[2], 0, 0);
    }

    public void Load()
    {
      Pages.ForEach(l_FormPage => l_FormPage.Load());
      m_FormPages[0].Activate();
    }

    public virtual void Unload()
    {
      Pages.ForEach(l_FormPage => l_FormPage.Unload());
    }

    public void RotateLeft()
    {
      if (m_FormPageIndex < 0)
        m_FormPageIndex = m_FormPages.Length - 1;
      SwitchToPage(m_FormPages[m_FormPageIndex]);
    }

    public void RotateRight()
    {
      if (m_FormPageIndex > m_FormPages.Length - 1)
        m_FormPageIndex = 0;
      SwitchToPage(m_FormPages[m_FormPageIndex]);
    }

    private void FormPage_ActivateRequested(FormPage p_FormPage)
    {
      SwitchToPage(p_FormPage);
    }

    private void SwitchToPage(FormPage p_FormPage)
    {
      Pages.ForEach(l_FormPage =>
      {
        if (p_FormPage == l_FormPage)
          l_FormPage.Activate();
        else if (l_FormPage.Activated)
          l_FormPage.Deactivate();
      });
    }

    public FormPage? ActivateFormPage(Type p_FormPageType)
    {
      FormPage? v_FormPage = null;
      Pages.ForEach(l_FormPage =>
      {
        if (p_FormPageType == l_FormPage.Type)
        {
          l_FormPage.Activate();
          v_FormPage = l_FormPage;
        }
        else if (l_FormPage.Activated)
          l_FormPage.Deactivate();
      });
      return v_FormPage;
    }

    public T GetFormPage<T>() where T : class
    {
      foreach (var l_FormPage in Pages)
      {
        if (l_FormPage is T)
        {
          T? v_FormPage = l_FormPage as T;
          if (v_FormPage is not null)
            return v_FormPage;
        }
      }
      throw new InvalidOperationException($"GetFormPage of type {typeof(T)} does not exist!");
    }
  }
}