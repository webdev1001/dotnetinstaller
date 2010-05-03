using System;
using System.Xml;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;

namespace InstallerLib
{
    /// <summary>
    /// A base component.
    /// </summary>
    [XmlChild(typeof(InstalledCheck))]
    [XmlChild(typeof(InstalledCheckOperator))]
    [XmlChild(typeof(DownloadDialog), Max = 1)]
    [XmlChild(typeof(EmbedFile))]
    [XmlChild(typeof(EmbedFolder))]
    public abstract class Component : XmlClass
    {
        public Component(string type, string name)
        {
            m_type = type;
            Template.Template_component tpl = Template.CurrentTemplate.component(name);
            m_display_name = tpl.display_name;
        }

        #region Attributes

        private string m_type;
        [Description("The type of the component; can be 'cmd' for executing generic command line installation or 'msi' for installing Windows Installer MSI package or 'openfile' to open a file. (REQUIRED)")]
        public string type
        {
            get { return m_type; }
        }

        private string m_os_filter;
        [Description("A filter to install this component only on all operating system equal or not equal the value(s) specified. Separate multiple operating system ids with comma (',') and use not symbol ('!') for NOT logic (es. '44,!45' ).")]
        [Category("Operating System")]
        public string os_filter
        {
            get { return m_os_filter; }
            set { m_os_filter = value; }
        }

        private OperatingSystem m_os_filter_min;
        [Description("A filter to install this component only on all operating system id greater or equal than the id specified. For example to install a component only in Windows 2000 or later use win2000. (OPTIONAL)")]
        [Category("Operating System")]        
        public OperatingSystem os_filter_min
        {
            get { return m_os_filter_min; }
            set { m_os_filter_min = value; }
        }

        private OperatingSystem m_os_filter_max;
        [Description("A filter to install this component only on all operating system id smaller or equal than the id specified. For example to install a component preceding Windows 2000 use winNT4sp6a. (OPTIONAL)")]
        [Category("Operating System")]
        public OperatingSystem os_filter_max
        {
            get { return m_os_filter_max; }
            set { m_os_filter_max = value; }
        }

        private string m_os_filter_lcid;
        [Description("A filter to install this component only on all operating system language equals or not equals than the LCID specified (see Help->LCID table). Separate multiple LCID with comma (',') and use not symbol ('!') for NOT logic (es. '1044,1033,!1038' ). You can also filter all the configuration element. (OPTIONAL)")]
        [Category("Operating System")]
        public string os_filter_lcid
        {
            get { return m_os_filter_lcid; }
            set { m_os_filter_lcid = value; }
        }

        private string m_installcompletemessage; //se vuoto non viene visualizzato nessun messagio al termine del download
        [Description("The message used when a component is successfully installed. To disable this message leave this property empty. (OPTIONAL)")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Category("Install")]
        public string installcompletemessage
        {
            get { return m_installcompletemessage; }
            set { m_installcompletemessage = value; }
        }

        private string m_uninstallcompletemessage;
        [Description("The message used when a component is successfully uninstalled. (OPTIONAL)")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Category("Uninstall")]
        public string uninstallcompletemessage
        {
            get { return m_uninstallcompletemessage; }
            set { m_uninstallcompletemessage = value; }
        }

        private bool m_mustreboot = false;
        [Description("Indicates whether to reboot automatically after this component is installed or uninstalled successfully. Normally if the system must be restarted, the component tells this setup (with special return code) to stop and restart the system. This forces a reboot without prompting. (REQUIRED)")]
        public bool mustreboot
        {
            get { return m_mustreboot; }
            set { m_mustreboot = value; }
        }

        private string m_failed_exec_command_continue;
        [Description("The message used when a component cannot be installed and ask if the application can continue with others components (Yes/No message). Must contain one '%s' string where the application put the description of the component. (OPTIONAL)")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string failed_exec_command_continue
        {
            get { return m_failed_exec_command_continue; }
            set { m_failed_exec_command_continue = value; }
        }

        private string m_reboot_required;
        [Description("The message used when this component signaled the installer that it requires a reboot. (OPTIONAL)")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string reboot_required
        {
            get { return m_reboot_required; }
            set { m_reboot_required = value; }
        }

        private bool m_must_reboot_required = false;
        [Description("Component setting for reboot behavior. When true, installation won't continue after this component required a reboot. (REQUIRED)")]
        public bool must_reboot_required
        {
            get { return m_must_reboot_required; }
            set { m_must_reboot_required = value; }
        }

        private bool m_allow_continue_on_error = true;
        [Description("If true, the user will be prompted to continue when a component fails to install. (REQUIRED)")]
        public bool allow_continue_on_error
        {
            get { return m_allow_continue_on_error; }
            set { m_allow_continue_on_error = value; }
        }

        private string m_id;
        [Description("Component identity. This value should be unique. (REQUIRED)")]
        [Category("Component")]
        public string id
        {
            get { return m_id; }
            set { m_id = value; OnIdChanged(); }
        }

        private string m_display_name;
        [Description("Display name of this component. This value is used also in some message to replace the %s string. (REQUIRED)")]
        [Category("Component")]
        public string display_name
        {
            get { return m_display_name; }
            set { m_display_name = value; OnDisplayNameChanged(); }
        }

        private string m_uninstall_display_name;
        [Description("Optional display name of this component during uninstall.")]
        [Category("Component")]
        public string uninstall_display_name
        {
            get { return m_uninstall_display_name; }
            set { m_uninstall_display_name = value; OnDisplayNameChanged(); }
        }

        private bool m_required = false;
        [Description("Indicates whether the component is required for a successful installation or uninstallation. (REQUIRED)")]
        [Category("Component")]
        public bool required
        {
            get { return m_required; }
            set { m_required = value; }
        }

        private bool m_selected = true;
        [Description("Indicates whether the component is selected by default.")]
        [Category("Component")]
        public bool selected
        {
            get { return m_selected; }
            set { m_selected = value; }
        }

        private string m_note;
        [Description("Note, not used by the setup. (OPTIONAL)")]
        public string note
        {
            get { return m_note; }
            set { m_note = value; }
        }

        // message for not matching the processor architecture filter
        private string m_processor_architecture_filter;
        [Description("Type of processor architecture (x86, mips, alpha, ppc, shx, arm, ia64, alpha64, msil, x64, ia32onwin64). Seperate by commas, can use the NOT sign ('!') to exclude. (es. 'x86,x64' or '!x86'). (OPTIONAL)")]
        [Category("Filters")]
        public string processor_architecture_filter
        {
            get { return m_processor_architecture_filter; }
            set { m_processor_architecture_filter = value; }
        }

        private string m_status_installed;
        [Description("The string used for indicating that this component is installed. (OPTIONAL)")]
        public string status_installed
        {
            get { return m_status_installed; }
            set { m_status_installed = value; }
        }

        private string m_status_notinstalled;
        [Description("The string used for indicating that this component is not installed. (OPTIONAL)")]
        public string status_notinstalled
        {
            get { return m_status_notinstalled; }
            set { m_status_notinstalled = value; }
        }

        private bool m_supports_install = true;
        [Description("Indicates whether the component supports the install sequence. (REQUIRED)")]
        [Category("Install")]
        public bool supports_install
        {
            get { return m_supports_install; }
            set { m_supports_install = value; }
        }

        private bool m_supports_uninstall = false;
        [Description("Indicates whether the component supports the uninstall sequence. (REQUIRED)")]
        [Category("Uninstall")]
        public bool supports_uninstall
        {
            get { return m_supports_uninstall; }
            set { m_supports_uninstall = value; }
        }

        #endregion

        #region Events

        protected void OnDisplayNameChanged()
        {
            if (DisplayNameChanged != null)
            {
                DisplayNameChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler DisplayNameChanged;

        protected void OnIdChanged()
        {
            if (IdChanged != null)
            {
                IdChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler IdChanged;

        #endregion

        #region XmlClass Members

        public override string XmlTag
        {
            get { return "component"; }
        }

        #endregion

        protected override void OnXmlWriteTag(XmlWriterEventArgs e)
        {
            e.XmlWriter.WriteAttributeString("id", string.IsNullOrEmpty(m_id) ? m_display_name : m_id);
            e.XmlWriter.WriteAttributeString("display_name", m_display_name);
            e.XmlWriter.WriteAttributeString("uninstall_display_name", m_uninstall_display_name);
            e.XmlWriter.WriteAttributeString("os_filter", m_os_filter);
            e.XmlWriter.WriteAttributeString("os_filter_min", (m_os_filter_min == OperatingSystem.winNone
                ? "" : Enum.GetName(typeof(OperatingSystem), m_os_filter_min)));
            e.XmlWriter.WriteAttributeString("os_filter_max", (m_os_filter_max == OperatingSystem.winNone
                ? "" : Enum.GetName(typeof(OperatingSystem), m_os_filter_max)));
            e.XmlWriter.WriteAttributeString("os_filter_lcid", m_os_filter_lcid);
            e.XmlWriter.WriteAttributeString("type", m_type);
            e.XmlWriter.WriteAttributeString("installcompletemessage", m_installcompletemessage);
            e.XmlWriter.WriteAttributeString("uninstallcompletemessage", m_uninstallcompletemessage);
            e.XmlWriter.WriteAttributeString("mustreboot", m_mustreboot.ToString());
            e.XmlWriter.WriteAttributeString("reboot_required", m_reboot_required);
            e.XmlWriter.WriteAttributeString("must_reboot_required", m_must_reboot_required.ToString());
            e.XmlWriter.WriteAttributeString("failed_exec_command_continue", m_failed_exec_command_continue);            
            e.XmlWriter.WriteAttributeString("allow_continue_on_error", m_allow_continue_on_error.ToString());
            e.XmlWriter.WriteAttributeString("required", m_required.ToString());
            e.XmlWriter.WriteAttributeString("selected", m_selected.ToString());
            e.XmlWriter.WriteAttributeString("note", m_note);
            e.XmlWriter.WriteAttributeString("processor_architecture_filter", m_processor_architecture_filter);
            e.XmlWriter.WriteAttributeString("status_installed", m_status_installed);
            e.XmlWriter.WriteAttributeString("status_notinstalled", m_status_installed);
            e.XmlWriter.WriteAttributeString("supports_install", m_supports_install.ToString());
            e.XmlWriter.WriteAttributeString("supports_uninstall", m_supports_uninstall.ToString());
            base.OnXmlWriteTag(e);
        }

        protected override void OnXmlReadTag(XmlElementEventArgs e)
        {
            // backwards compatible description < 1.8
            ReadAttributeValue(e, "description", ref m_display_name);
            ReadAttributeValue(e, "display_name", ref m_display_name);
            ReadAttributeValue(e, "uninstall_display_name", ref m_uninstall_display_name);
            ReadAttributeValue(e, "id", ref m_id);
            if (string.IsNullOrEmpty(m_id)) m_id = m_display_name;
            ReadAttributeValue(e, "installcompletemessage", ref m_installcompletemessage);
            ReadAttributeValue(e, "uninstallcompletemessage", ref m_uninstallcompletemessage);
            ReadAttributeValue(e, "mustreboot", ref m_mustreboot);
            ReadAttributeValue(e, "reboot_required", ref m_reboot_required);
            ReadAttributeValue(e, "must_reboot_required", ref m_must_reboot_required);
            ReadAttributeValue(e, "failed_exec_command_continue", ref m_failed_exec_command_continue);
            ReadAttributeValue(e, "allow_continue_on_error", ref m_allow_continue_on_error);
            ReadAttributeValue(e, "required", ref m_required);
            ReadAttributeValue(e, "selected", ref m_selected);
            ReadAttributeValue(e, "os_filter", ref m_os_filter);
            ReadAttributeValue(e, "os_filter_lcid", ref m_os_filter_lcid);
            string os_filter_greater = string.Empty;
            if (ReadAttributeValue(e, "os_filter_greater", ref os_filter_greater) && !String.IsNullOrEmpty(os_filter_greater))
                m_os_filter_min = (OperatingSystem)(int.Parse(os_filter_greater) + 1);
            string os_filter_smaller = string.Empty;
            if (ReadAttributeValue(e, "os_filter_smaller", ref os_filter_smaller) && !String.IsNullOrEmpty(os_filter_smaller))
                m_os_filter_max = (OperatingSystem)(int.Parse(os_filter_smaller) - 1);
            ReadAttributeValue(e, "os_filter_min", ref m_os_filter_min);
            ReadAttributeValue(e, "os_filter_max", ref m_os_filter_max);
            ReadAttributeValue(e, "note", ref m_note);
            ReadAttributeValue(e, "processor_architecture_filter", ref m_processor_architecture_filter);
            ReadAttributeValue(e, "status_installed", ref m_status_installed);
            ReadAttributeValue(e, "status_notinstalled", ref m_status_notinstalled);
            ReadAttributeValue(e, "supports_install", ref m_supports_install);
            ReadAttributeValue(e, "supports_uninstall", ref m_supports_uninstall);
            base.OnXmlReadTag(e);
        }

        public static Component CreateFromXml(XmlElement element)
        {
            Component l_Comp;
            string xmltype = element.Attributes["type"].InnerText;
            if (xmltype == "msi")
                l_Comp = new ComponentMsi();
            else if (xmltype == "msu")
                l_Comp = new ComponentMsu();
            else if (xmltype == "msp")
                l_Comp = new ComponentMsp();
            else if (xmltype == "cmd")
                l_Comp = new ComponentCmd();
            else if (xmltype == "openfile")
                l_Comp = new ComponentOpenFile();
            else if (xmltype == "exe")
                l_Comp = new ComponentExe();
            else
                throw new Exception(string.Format("Invalid type: {0}", xmltype));

            l_Comp.FromXml(element);

            return l_Comp;
        }

        /// <summary>
        /// Return files to embed for this component.
        /// </summary>
        /// <param name="parentid"></param>
        /// <param name="supportdir"></param>
        /// <returns></returns>
        public override Dictionary<string, EmbedFileCollection> GetFiles(string parentid, string supportdir)
        {
            return base.GetFiles(id, supportdir);
        }
    }
}
