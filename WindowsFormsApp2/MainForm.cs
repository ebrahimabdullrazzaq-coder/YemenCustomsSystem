using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    // ==================== ✅ كلاس إدارة اللغات ====================
    public static class LanguageManager
    {
        private static string _currentLanguage = "ar";
        private static Dictionary<string, Dictionary<string, string>> _translations;
        private static string _settingsPath = Path.Combine(Application.StartupPath, "app_settings.json");

        public static event EventHandler LanguageChanged;

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    SaveSettings();
                    LanguageChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static bool IsRTL => _currentLanguage == "ar";
        public static RightToLeft RightToLeft => IsRTL ? RightToLeft.Yes : RightToLeft.No;

        public static void Initialize()
        {
            LoadTranslations();
            LoadSettings();
        }

        private static void LoadTranslations()
        {
            var arabic = new Dictionary<string, string>
            {
                ["menu_setup"] = "⚙️ تهيئة النظام",
                ["menu_operations"] = "📋 العمليات",
                ["menu_reports"] = "📊 التقارير",
                ["menu_exit"] = "🚪 خروج",
                ["menu_currencies"] = "💱 تهيئة العملات",
                ["menu_ports"] = "🚢 تهيئة المنافذ",
                ["menu_warehouses"] = "🏢 تهيئة المخازن",
                ["menu_items"] = "📦 تهيئة الأصناف",
                ["menu_items_data"] = "الترسيم الجمركي",
                ["menu_customs_clearance"] = "🛃 شاشة التخليص الجمركي",
                ["menu_stock_transfer"] = "🚚 نقل من المنفذ إلى المخزن",
                ["menu_warehouse_transfer"] = "🏚️ نقل بين المخازن",
                ["menu_currency_reports"] = "📈 تقارير العملات",
                ["menu_clearance_reports"] = "📋 تقارير التخليص",
                ["menu_warehouse_reports"] = "🏚️ تقارير حركة المخازن",
                ["menu_users_management"] = "👥 إدارة المستخدمين",
                ["welcome_title"] = "نظام الجمارك اليمنية المتكامل",
                ["msg_confirm_exit"] = "هل تريد الخروج من النظام؟",
                ["msg_in_development"] = "قيد التطوير",
                ["language_menu"] = "🌐 اللغة / Language",
                ["lang_arabic"] = "🇾🇪 العربية",
                ["lang_english"] = "🇬🇧 English",
                ["btn_logout"] = "🚪 تسجيل الخروج",
                ["msg_confirm_logout"] = "🔐 هل تريد تسجيل الخروج من النظام؟",
                ["msg_warning"] = "⚠️ تنبيه",
                ["msg_success"] = "✅ نجاح",
                ["msg_error"] = "❌ خطأ",
                ["msg_confirm"] = "تأكيد",
                ["msg_no_permission"] = "🚫 ليس لديك صلاحية الوصول إلى هذه الشاشة",
                ["msg_contact_admin"] = "تواصل مع المدير للحصول على الصلاحية المطلوبة",
                ["btn_home"] = "🏠 الرئيسية",
                ["btn_save"] = "💾 حفظ",
                ["btn_new"] = "🆕 جديد",
                ["btn_edit"] = "✏️ تعديل",
                ["btn_delete"] = "🗑️ حذف",
                ["btn_search"] = "🔍 بحث",
                ["btn_export"] = "📤 تصدير",
                ["btn_exit"] = "🚪 خروج",
                ["btn_print"] = "🖨️ طباعة",
                ["btn_filter"] = "🔍 تصفية",
                ["btn_view"] = "👁️ عرض",
                ["btn_first"] = "⏮ الأول",
                ["btn_previous"] = "◀ السابق",
                ["btn_next"] = "التالي ▶",
                ["btn_last"] = "الأخير ⏭",
                ["btn_units"] = "📏 وحدات القياس",
                ["btn_load_items"] = "📥 إنزال الأصناف",
                ["btn_save_data"] = "💾 حفظ البيانات",
                ["btn_movements"] = "📜 سجل الحركات",
                ["btn_print_report"] = "🖨️ طباعة التقرير",
                ["btn_all_data"] = "📋 قائمة جميع البيانات",
                ["btn_load_statement"] = "🔍 تحميل البيان",
                ["btn_select"] = "✅ اختيار",
                ["btn_cancel"] = "❌ إلغاء",
                ["btn_select_statement"] = "✅ اختيار البيان المحدد",
                ["btn_close"] = "❌ إغلاق",
                ["btn_choose_port"] = "-- اختر منفذ --",
                ["btn_no_ports"] = "-- لا توجد منافذ --",
                ["btn_loading"] = "-- جاري التحميل --",
                ["btn_load_error"] = "-- خطأ في التحميل --",
                ["items_form_title"] = "الترسيم الجمركي",
                ["items_form_nav"] = "الترسيم الجمركي - {0}/{1}",
                ["port_classification"] = "تصنيف المنفذ:",
                ["port_name"] = "اسم المنفذ:",
                ["all"] = "-- الكل --",
                ["internal"] = "داخلي (مناطق الشمال)",
                ["external"] = "خارجي (مناطق الجنوب)",
                ["status_select_classification"] = "📌 اختر تصنيف المنفذ ثم اضغط على 'إنزال الأصناف'",
                ["status_no_data"] = "⚠️ لا توجد بيانات لهذا التصنيف",
                ["status_loaded"] = "✅ تم تحميل {0} صنف",
                ["status_loaded_items"] = "📊 تم تحميل {0} صنف مسعر + {1} صنف جديد للمنافذ {2}",
                ["msg_select_classification"] = "⚠️ الرجاء اختيار تصنيف المنفذ أولاً",
                ["msg_no_items"] = "❌ لا توجد أصناف في شاشة تهيئة الأصناف",
                ["msg_items_loaded"] = "✅ تم إنزال {0} صنف\n📌 الأصناف المسعرة: {1}\n🆕 الأصناف الجديدة: {2}",
                ["msg_new_items"] = "✅ تم إنزال {0} صنف جديد",
                ["msg_saved"] = "✅ تم حفظ {0} صنف بنجاح",
                ["msg_select_row"] = "⚠️ الرجاء تحديد السجل أولاً",
                ["msg_no_data_search"] = "️ لا توجد بيانات للبحث",
                ["msg_enter_product_name"] = "أدخل اسم المنتج للبحث:",
                ["msg_found"] = "✅ تم العثور على: {0}",
                ["msg_not_found"] = "❌ لم يتم العثور على المنتج",
                ["msg_no_data_export"] = "لا توجد بيانات للتصدير",
                ["msg_exported"] = "✅ تم تصدير {0} سجل بنجاح",
                ["col_id"] = "م",
                ["col_product_name"] = "اسم السلعة",
                ["col_weight"] = "الوزن",
                ["col_customs_price_usd"] = "السعر الجمركي بالدولار",
                ["col_exchange_rate"] = "سعر الصرف",
                ["col_customs_price_ry"] = "السعر الجمركي بالريال",
                ["col_customs_rate_pct"] = "نسبة الرسوم",
                ["col_customs_value"] = "القيمة الجمركية",
                ["col_st_pct"] = "ض/المبيعات ST",
                ["col_vat_pct"] = "ض/القيمة المضافة VAT",
                ["col_pt_pct"] = "ض/الأرباح PT",
                ["col_vf_pct"] = "تحسين VF",
                ["col_tef_pct"] = "معلم TEF",
                ["col_mff_pct"] = "الشهيد MFF",
                ["col_total_customs"] = "إجمالي الرسوم",
                ["col_total_st"] = "إجمالي ST",
                ["col_total_vat"] = "إجمالي VAT",
                ["col_total_pt"] = "إجمالي PT",
                ["col_total_vf"] = "إجمالي VF",
                ["col_total_tef"] = "إجمالي TEF",
                ["col_total_mff"] = "إجمالي MFF",
                ["item_form_title"] = "بيانات الأصناف",
                ["grp_header"] = "بيانات الصنف",
                ["lbl_item_image"] = "صورة الصنف:",
                ["lbl_item_type"] = "نوع الصنف:",
                ["lbl_group_no"] = "رقم المجموعة ⭐:",
                ["lbl_group_name"] = "اسم المجموعة ⭐:",
                ["lbl_sub_group_under"] = "رقم م. تحت الفرعية:",
                ["lbl_item_code"] = "رقم الصنف ⭐:",
                ["lbl_item_name"] = "اسم الصنف ⭐:",
                ["lbl_branch"] = "م. الفرعية:",
                ["lbl_ref_no"] = "رقم المرجع:",
                ["lbl_barcode"] = "رقم الباركود:",
                ["lbl_foreign_name"] = "الاسم الأجنبي:",
                ["tab_main"] = "البيانات الرئيسية",
                ["tab_units"] = "وحدات الصنف",
                ["tab_other"] = "بيانات أخرى",
                ["tab_cost"] = "التكلفة",
                ["tab_movement"] = "الحركة",
                ["tab_analysis"] = "بيانات تحليلية",
                ["tab_prices"] = "الأسعار",
                ["tab_weight_units"] = "وحدات الوزن",
                ["lbl_item_specs"] = "مواصفات الصنف:",
                ["lbl_item_specs_foreign"] = "المواصفات بالأجنبي:",
                ["lbl_assistant"] = "م. المساعدة:",
                ["lbl_detail_group"] = "م. التفصيلية:",
                ["lbl_class"] = "التصنيف:",
                ["lbl_initial_cost"] = "التكلفة الأولية:",
                ["lbl_default_unit"] = "الوحدة الافتراضية:",
                ["lbl_purchase_invoice"] = "رقم فاتورة الشراء:",
                ["lbl_item_activity"] = "نشاط الصنف:",
                ["lbl_return_period"] = "فترة الارتجاع:",
                ["lbl_rank"] = "الرتبة:",
                ["lbl_length"] = "الطول:",
                ["lbl_size"] = "المقاس:",
                ["lbl_width"] = "العرض:",
                ["lbl_color"] = "اللون:",
                ["lbl_height"] = "الارتفاع:",
                ["lbl_season"] = "الموسم:",
                ["lbl_volume"] = "الحجم:",
                ["lbl_material"] = "الخامة:",
                ["lbl_area"] = "المساحة:",
                ["lbl_brand"] = "الماركة:",
                ["lbl_manufacturer"] = "الشركة المصنعة:",
                ["lbl_mfg_no"] = "رقم التصنيع:",
                ["lbl_country_main"] = "بلد الصنع:",
                ["lbl_storage"] = "تخزين الاصناف:",
                ["lbl_open_cost"] = "تكلفة بداية التعامل:",
                ["lbl_stop_date"] = "تاريخ التوقيف:",
                ["lbl_opening_stock"] = "المخزون الافتتاحي:",
                ["lbl_stop_reason"] = "سبب التوقيف:",
                ["lbl_available_qty"] = "الكمية المتوفرة:",
                ["col_unit_num"] = "م",
                ["col_unit_name"] = "الوحدة",
                ["col_unit_package"] = "العبوة",
                ["col_weight_unit"] = "الوحدة",
                ["col_weight_value"] = "الوزن",
                ["col_cost_warehouse"] = "المخزن",
                ["col_cost_warehouse_name"] = "اسم المخزن",
                ["col_cost_initial"] = "التكلفة الأولية",
                ["col_cost_avg"] = "متوسط التكلفه",
                ["col_cost_available"] = "الكمية المتوفرة",
                ["col_cost_status"] = "موقف",
                ["chk_allow_free_qty"] = "يسمح بكمية مجانية",
                ["chk_has_warranty"] = "علية ضمان",
                ["lbl_warranty_period"] = "فترة الضمان:",
                ["lbl_warranty_day"] = "يوم",
                ["chk_has_commission"] = "له عمولة",
                ["lbl_commission_type"] = "نوع العمولة:",
                ["lbl_free_qty_percent"] = "نسبة الكمية المجانية:",
                ["lbl_expiry_duration"] = "مدة إنتهاء الصنف:",
                ["lbl_expiry_day"] = "يوم",
                ["lbl_min_cost_percent"] = "نسبة الحد الأدنى للتكلفة:",
                ["lbl_max_cost_percent"] = "نسبة الحد الأعلى للتكلفة:",
                ["chk_is_weighted"] = "صنف موزن",
                ["lbl_scale_name"] = "اسم الميزان:",
                ["lbl_key"] = "المفتاح:",
                ["lbl_doc_type"] = "نوع رقم الوثيقة:",
                ["lbl_doc_ref_no"] = "رقم الوثيقة المرجع:",
                ["lbl_doc_seq"] = "تسلسل الوثيقة:",
                ["item_type_goods"] = "بضاعة",
                ["item_type_service"] = "خدمة",
                ["item_type_asset"] = "أصل ثابت",
                ["item_activity_active"] = "نشط",
                ["item_activity_inactive"] = "غير نشط",
                ["item_activity_temp"] = "مؤقت",
                ["item_activity_stopped"] = "متوقف",
                ["rank_a"] = "أ",
                ["rank_b"] = "ب",
                ["rank_c"] = "ج",
                ["rank_d"] = "د",
                ["rank_e"] = "هـ",
                ["season_summer"] = "صيف",
                ["season_winter"] = "شتاء",
                ["season_spring"] = "ربيع",
                ["season_autumn"] = "خريف",
                ["season_all"] = "جميع المواسم",
                ["season_none"] = "غير محدد",
                ["default_unit_piece"] = "قطعة",
                ["default_unit_kg"] = "كجم",
                ["default_unit_liter"] = "لتر",
                ["default_unit_carton"] = "كرتون",
                ["default_unit_can"] = "علبة",
                ["msg_group_required"] = "⚠️ رقم المجموعة مطلوب",
                ["msg_group_name_required"] = "⚠️ اسم المجموعة مطلوب",
                ["msg_item_code_required"] = "⚠️ رقم الصنف مطلوب",
                ["msg_item_name_required"] = "⚠️ اسم الصنف مطلوب",
                ["msg_update_success"] = "✅ تم تحديث الصنف بنجاح",
                ["msg_add_success"] = "✅ تم إضافة الصنف بنجاح",
                ["msg_delete_confirm"] = "🗑️ حذف:\n{0} - {1}؟",
                ["msg_delete_title"] = "تأكيد",
                ["msg_delete_success"] = "✅ تم الحذف",
                ["msg_no_item"] = "️ لا يوجد صنف محدد",
                ["msg_search"] = "عرض",
                ["msg_search_label"] = "أدخل رقم أو اسم الصنف:",
                ["msg_btn_search"] = "بحث",
                ["msg_select_unit"] = "🔍 اختر وحدة القياس (انقر مرتين أو اضغط Enter)",
                ["msg_add_image"] = "اختر صورة الصنف",
                ["msg_printing"] = "🖨️ جاري الطباعة...",
                ["msg_exporting"] = "📊 جاري التصدير...",
                ["msg_filtering"] = "🔍 جاري التصفية...",
                ["success_title"] = "نجاح",
                ["search_title"] = "بحث",
                ["currencies_form_title"] = "💱 تهيئة العملات",
                ["grp_currency_data"] = " بيانات العملة",
                ["grp_audit_log"] = "📊 سجل التعديلات",
                ["lbl_currency_number"] = "رقم العملة:",
                ["lbl_currency_name"] = "اسم العملة:",
                ["lbl_currency_symbol"] = "الرمز:",
                ["lbl_currency_type"] = "نوع العملة:",
                ["lbl_exchange_rate"] = "سعر التحويل:",
                ["lbl_min_rate"] = "الحد الأدنى:",
                ["lbl_max_rate"] = "الحد الأقصى:",
                ["lbl_foreign_name"] = "اسم العملة الأجنبية:",
                ["lbl_subunit"] = "الوحدة الفرعية:",
                ["lbl_decimal_places"] = "الخانات العشرية:",
                ["radio_local"] = "محلية",
                ["radio_foreign"] = "أجنبية",
                ["chk_stored"] = "مخزنة",
                ["col_currency"] = "العملة",
                ["col_action"] = "الإجراء",
                ["col_old_rate"] = "السعر القديم",
                ["col_new_rate"] = "السعر الجديد",
                ["col_old_min_rate"] = "أدنى سعر قديم",
                ["col_new_min_rate"] = "أدنى سعر جديد",
                ["col_old_max_rate"] = "أعلى سعر قديم",
                ["col_new_max_rate"] = "أعلى سعر جديد",
                ["col_user"] = "المستخدم",
                ["col_modified_date"] = "التاريخ والوقت",
                ["msg_currency_number_required"] = "️ رقم العملة مطلوب",
                ["msg_currency_name_required"] = "⚠️ اسم العملة مطلوب",
                ["msg_invalid_rate"] = "️ سعر تحويل غير صحيح",
                ["msg_save_success"] = "✅ تم الحفظ بنجاح",
                ["msg_edit_success"] = "✅ تم التعديل بنجاح",
                ["msg_add_success_curr"] = "✅ تمت الإضافة بنجاح",
                ["msg_delete_success_curr"] = "✅ تم الحذف",
                ["msg_select_record"] = "⚠️ الرجاء اختيار سجل للتعديل",
                ["msg_select_currency"] = "⚠️ الرجاء تحديد العملة أولاً",
                ["msg_enter_currency_name"] = "⚠️ أدخل اسم العملة للبحث",
                ["msg_confirm_delete_curr"] = "️ حذف « {0} » نهائياً؟",
                ["msg_currency_not_found"] = "️ لم يتم العثور على العملة",
                ["msg_not_found_curr"] = "❌ لم يتم العثور على: {0}",
                ["action_add"] = "إضافة",
                ["action_edit"] = "تعديل",
                ["action_delete"] = "حذف",
                ["type_local"] = "محلية",
                ["type_foreign"] = "أجنبية",
                ["print_title"] = "تقرير سجل تعديلات العملات - نظام الجمارك اليمني",
                ["print_date"] = "تاريخ الطباعة: {0}",
                ["ports_form_title"] = "🏛️ تهيئة المنافذ - نظام الجمارك اليمنية",
                ["grp_port_data"] = "🏛️ بيانات المنفذ",
                ["grp_classification"] = "🗂️ التصنيف",
                ["grp_nature"] = "🌍 الطبيعة",
                ["grp_rate"] = "💰 سعر الصرف الجمركي",
                ["grp_payment"] = "💳 طريقة الدفع",
                ["lbl_port_number"] = "رقم المنفذ:",
                ["lbl_port_name"] = "اسم المنفذ:",
                ["lbl_port_address"] = "عنوان المنفذ:",
                ["lbl_port_type"] = "نوع المنفذ:",
                ["lbl_currency"] = "العملة:",
                ["radio_internal"] = "داخلي من مناطق الشمال",
                ["radio_external"] = "خارجي من مناطق الجنوب",
                ["radio_land"] = "بري",
                ["radio_air"] = "جوي",
                ["radio_sea"] = "بحري",
                ["radio_rate1"] = "250.5 ريال/دولار",
                ["radio_rate2_north"] = "535 ريال/دولار",
                ["radio_rate2_south"] = "750 ريال/دولار",
                ["radio_cash"] = "نقد",
                ["radio_check"] = "شيك",
                ["port_type_main"] = "رئيسي",
                ["port_type_supervisory"] = "رقابي",
                ["currency_yer"] = "ريال يمني",
                ["currency_yer_south"] = "ريال جنوبي",
                ["rate_info_north"] = "💰 السعر الجمركي: 250.5 أو 535 ريال يمني (شمالي)",
                ["rate_info_south"] = "💰 السعر الجمركي: 250.5 أو 750 ريال جنوبي (جنوبي)",
                ["col_port_name"] = "اسم المنفذ",
                ["col_port_nature"] = "طبيعة المنفذ",
                ["col_port_type"] = "نوع المنفذ",
                ["col_classification"] = "تصنيف المنفذ",
                ["col_exchange_rate_type"] = "سعر الصرف الجمركي",
                ["col_operation_type"] = "نوع العملية",
                ["msg_port_name_required"] = "️ اسم المنفذ مطلوب",
                ["msg_select_port_edit"] = "الرجاء اختيار منفذ للتعديل",
                ["msg_select_port_delete"] = "الرجاء اختيار المنفذ المراد حذفه",
                ["msg_confirm_delete_port"] = "️ هل أنت متأكد من حذف « {0} » نهائياً؟",
                ["msg_save_success_port"] = "✅ تم حفظ بيانات المنفذ بنجاح",
                ["msg_edit_success_port"] = "✅ تم تعديل بيانات المنفذ بنجاح",
                ["msg_delete_success_port"] = "✅ تم حذف المنفذ بنجاح",
                ["msg_no_ports"] = "لا توجد منافذ للتنقل",
                ["msg_first_port"] = "⚠️ أنت في أول منفذ",
                ["msg_last_port"] = "️ أنت في آخر منفذ",
                ["msg_enter_port_name"] = "الرجاء إدخال اسم المنفذ للبحث",
                ["msg_found_port"] = "✅ تم العثور على: {0}",
                ["msg_not_found_port"] = "❌ لم يتم العثور على منفذ يحتوي على: {0}",
                ["msg_error_loading"] = "خطأ في تحميل البيانات: {0}",
                ["search_result"] = "نتيجة البحث",
                ["warehouses_form_title"] = "🏢 تهيئة المخازن - نظام الجمارك اليمنية",
                ["grp_warehouse_data"] = "📦 بيانات المخزن",
                ["lbl_warehouse_code"] = "رمز المخزن:",
                ["lbl_warehouse_name"] = "اسم المخزن:",
                ["lbl_warehouse_type"] = "نوع المخزن:",
                ["lbl_port_link"] = "المنفذ المرتبط:",
                ["lbl_address"] = "العنوان:",
                ["lbl_manager"] = "المسؤول:",
                ["lbl_phone"] = "رقم الهاتف:",
                ["lbl_status"] = "الحالة:",
                ["lbl_notes"] = "ملاحظات:",
                ["not_linked"] = "-- غير مرتبط --",
                ["type_main"] = "رئيسي",
                ["type_sub"] = "فرعي",
                ["type_temp"] = "مؤقت",
                ["status_active"] = "نشط",
                ["status_inactive"] = "غير نشط",
                ["col_warehouse_code"] = "رمز المخزن",
                ["col_warehouse_name"] = "اسم المخزن",
                ["col_type"] = "النوع",
                ["col_port_link"] = "المنفذ المرتبط",
                ["col_address"] = "العنوان",
                ["col_manager"] = "المسؤول",
                ["col_phone"] = "الهاتف",
                ["col_status"] = "الحالة",
                ["msg_warehouse_code_required"] = "⚠️ رمز المخزن مطلوب",
                ["msg_warehouse_name_required"] = "⚠️ اسم المخزن مطلوب",
                ["msg_select_warehouse_edit"] = "⚠️ الرجاء اختيار مخزن للتعديل أولاً",
                ["msg_select_warehouse_delete"] = "⚠️ الرجاء اختيار مخزن للحذف أولاً",
                ["msg_confirm_delete_warehouse"] = "️ هل أنت متأكد من حذف المخزن '{0}'؟",
                ["msg_delete_title_wh"] = "تأكيد الحذف",
                ["msg_edit_success_wh"] = "✏️ تم تعديل المخزن بنجاح",
                ["msg_add_success_wh"] = "✅ تم إضافة المخزن بنجاح",
                ["msg_delete_success_wh"] = "✅ تم حذف المخزن بنجاح",
                ["msg_not_found_wh"] = "❌ لم يتم العثور على المخزن",
                ["msg_found_wh"] = "✅ تم العثور على المخزن: {0}",
                ["msg_enter_search"] = "أدخل اسم أو رمز المخزن للبحث:",
                ["search_title_wh"] = "بحث",
                ["edit_title"] = "تعديل",
                ["add_title"] = "إضافة",
                ["delete_title"] = "حذف",
                ["export_title"] = "تصدير",
                ["clearance_form_title"] = "📋 شاشة التخليص الجمركي - نظام الجمارك اليمنية",
                ["lbl_request_number"] = "رقم طلب الترسيم:",
                ["lbl_statement_date"] = "تاريخ البيان:",
                ["lbl_statement_no"] = "رقم البيان:",
                ["lbl_client_name"] = "اسم العميل/الفرع:",
                ["lbl_car_no"] = "رقم السيارة:",
                ["lbl_total_fees"] = "💰 إجمالي الرسوم الجمركية والضرائب:",
                ["col_quantity"] = "الكمية ",
                ["col_transferred_qty"] = "الكمية المنقولة",
                ["col_remaining_qty"] = "الكمية المتبقية",
                ["col_stock_status"] = "حالة البضاعة",
                ["col_weight_total"] = "الوزن الإجمالي",
                ["col_customs_fee"] = "السعر الجمركي",
                ["col_customs_fee_total"] = "اجمالي السعر  الجمركي ",
                ["col_st"] = "ض/المبيعات st",
                ["col_st_total"] = "ض/المبيعات الإجمالي",
                ["col_vat"] = "ض/القيمه المضافه VAT",
                ["col_vat_total"] = "ض/القيمه المضافه الإجمالي",
                ["col_pt"] = "ض/ الارباح PT",
                ["col_pt_total"] = "ض/ الارباح الإجمالي",
                ["col_vf"] = "تحسين VF",
                ["col_vf_total"] = "تحسين الإجمالي",
                ["col_tef"] = "معلم TEF",
                ["col_tef_total"] = "معلم الإجمالي",
                ["col_mff"] = "الشهيد MFF",
                ["col_mff_total"] = "الشهيد الإجمالي",
                ["col_other"] = "أخرى",
                ["col_total"] = "الإجمالي النهائي",
                ["status_at_port"] = "📍 في المنفذ",
                ["status_transferred_full"] = "✅ تم النقل كلياً",
                ["status_transferred_partial"] = "🚚 تم النقل جزئياً",
                ["msg_enter_statement_no"] = "️ الرجاء إدخال رقم البيان",
                ["msg_no_data_for_statement"] = "❌ لا توجد بيانات لرقم البيان: {0}",
                ["msg_statement_loaded"] = "✅ تم تحميل البيان رقم {0} بنجاح",
                ["msg_enter_statement_first"] = "⚠️ الرجاء إدخال أو اختيار رقم البيان أولاً",
                ["msg_no_movements"] = "❌ لا توجد حركات تحويل لرقم البيان: {0}",
                ["movements_form_title"] = "📜 سجل حركات البضاعة - رقم البيان: {0}",
                ["col_date"] = "تاريخ التحويل",
                ["col_product"] = "اسم السلعة",
                ["col_qty"] = "الكمية المنقولة",
                ["col_from"] = "من",
                ["col_to"] = "إلى",
                ["col_transfer_no"] = "رقم التحويل",
                ["col_notes"] = "ملاحظات",
                ["msg_no_clearance_data"] = "📭 لا توجد بيانات تخليص مخزنة حالياً",
                ["statements_list_title"] = " قائمة بيانات التخليص الجمركي - اختر البيان",
                ["col_items_count"] = "عدد الأصناف",
                ["col_total_qty"] = "إجمالي الكمية",
                ["col_total_value"] = "الإجمالي النهائي",
                ["msg_select_port_first"] = "⚠️ الرجاء اختيار منفذ أولاً",
                ["msg_no_items_for_port"] = "❌ لا توجد أصناف مسجلة للمنافذ {0}ية",
                ["items_selection_title"] = "🔍 اختر الصنف - {0}",
                ["north_region"] = "مناطق الشمال",
                ["south_region"] = "مناطق الجنوب",
                ["msg_item_added"] = "✅ تم إضافة الصنف: {0}",
                ["msg_statement_no_required"] = "⚠️ رقم البيان مطلوب",
                ["msg_records_saved"] = "✅ تم حفظ {0} سجل بنجاح",
                ["msg_no_statement_to_delete"] = "⚠️ لا يوجد بيان محدد للحذف",
                ["msg_confirm_delete_statement"] = "🗑️ هل أنت متأكد من حذف البيان رقم {0}؟",
                ["msg_statement_deleted"] = "✅ تم حذف البيان بنجاح",
                ["msg_enter_statement_search"] = "أدخل رقم البيان للبحث:",
                ["msg_statement_found"] = "✅ تم العثور على البيان رقم {0}",
                ["msg_statement_not_found"] = "❌ لم يتم العثور على البيان",
                ["msg_export_success"] = "✅ تم تصدير البيانات بنجاح",
                ["msg_select_row_edit"] = "⚠️ الرجاء تحديد السجل المراد تعديله أولاً",
                ["msg_select_statement_first"] = "⚠️ الرجاء اختيار بيان أولاً",
                ["msg_statement_selected"] = "✅ تم تحميل البيان رقم: {0}",
                ["print_system_title"] = "نظام الجمارك اليمنية المتكامل",
                ["print_movements_title"] = "📜 سجل حركات البضاعة - رقم البيان: {0}",
                ["print_port_name"] = "اسم المنفذ: {0}",
                ["print_request_number"] = "رقم طلب الترسيم: {0}",
                ["print_statement_date"] = "تاريخ البيان: {0}",
                ["print_client_name"] = "اسم العميل/الفرع: {0}",
                ["print_car_no"] = "رقم السيارة: {0}",
                ["print_total_transferred"] = "إجمالي الكمية المنقولة: {0}",
                ["print_movements_count"] = "عدد الحركات: {0}",
                ["exchange_rate_selection_title"] = " اختيار سعر الصرف الجمركي",
                ["additional_amounts_form_title"] = "💰 المبالغ الإضافية في البيان",
                ["lbl_fee_no"] = "رقم الرسوم الإضافية:",
                ["lbl_fee_name"] = "اسم الرسوم الإضافية:",
                ["lbl_effect_type"] = "نوع التأثير:",
                ["effect_item"] = "مستوى الصنف",
                ["effect_statement"] = "مستوى البيان",
                ["effect_truck"] = "مستوى القاطرة",
                ["fee_by_port"] = "على حسب المنفذ",
                ["fee_by_port_group"] = "على حسب مجموعة المنافذ"
            };

            var english = new Dictionary<string, string>
            {
                ["menu_setup"] = "⚙️ System Setup",
                ["menu_operations"] = " Operations",
                ["menu_reports"] = "📊 Reports",
                ["menu_exit"] = "🚪 Exit",
                ["menu_currencies"] = "💱 Currencies Setup",
                ["menu_ports"] = "🚢 Ports Setup",
                ["menu_warehouses"] = " Warehouses Setup",
                ["menu_items"] = "📦 Items Setup",
                ["menu_items_data"] = "📋 Customs Tariff",
                ["menu_customs_clearance"] = " Customs Clearance",
                ["menu_stock_transfer"] = "🚚 Port to Warehouse Transfer",
                ["menu_warehouse_transfer"] = "🏚️ Warehouse Transfer",
                ["menu_currency_reports"] = "📈 Currency Reports",
                ["menu_clearance_reports"] = "📋 Clearance Reports",
                ["menu_warehouse_reports"] = "🏚️ Warehouse Reports",
                ["menu_users_management"] = "👥 Users Management",
                ["welcome_title"] = "Yemen Customs Integrated System",
                ["msg_confirm_exit"] = "Do you want to exit the system?",
                ["msg_in_development"] = "In Development",
                ["language_menu"] = " Language / اللغة",
                ["lang_arabic"] = "🇾🇪 العربية",
                ["lang_english"] = "🇬🇧 English",
                ["btn_logout"] = "🚪 Logout",
                ["msg_confirm_logout"] = "🔐 Do you want to logout?",
                ["msg_warning"] = "⚠️ Warning",
                ["msg_success"] = "✅ Success",
                ["msg_error"] = "❌ Error",
                ["msg_confirm"] = "Confirm",
                ["msg_no_permission"] = "🚫 You don't have permission to access this screen",
                ["msg_contact_admin"] = "Contact admin for required permission",
                ["btn_home"] = "🏠 Home",
                ["btn_save"] = "💾 Save",
                ["btn_new"] = "🆕 New",
                ["btn_edit"] = "✏️ Edit",
                ["btn_delete"] = "🗑️ Delete",
                ["btn_search"] = "🔍 Search",
                ["btn_export"] = "📤 Export",
                ["btn_exit"] = " Exit",
                ["btn_print"] = "️ Print",
                ["btn_filter"] = "🔍 Filter",
                ["btn_view"] = "👁️ View",
                ["btn_first"] = "⏮ First",
                ["btn_previous"] = "◀ Previous",
                ["btn_next"] = "Next ▶",
                ["btn_last"] = "Last ⏭",
                ["btn_units"] = "📏 Units",
                ["btn_load_items"] = "📥 Load Items",
                ["btn_save_data"] = "💾 Save Data",
                ["btn_movements"] = "📜 Movements Log",
                ["btn_print_report"] = "🖨️ Print Report",
                ["btn_all_data"] = " All Data List",
                ["btn_load_statement"] = "🔍 Load Statement",
                ["btn_select"] = "✅ Select",
                ["btn_cancel"] = "❌ Cancel",
                ["btn_select_statement"] = "✅ Select Statement",
                ["btn_close"] = "❌ Close",
                ["btn_choose_port"] = "-- Choose Port --",
                ["btn_no_ports"] = "-- No Ports --",
                ["btn_loading"] = "-- Loading --",
                ["btn_load_error"] = "-- Load Error --",
                ["items_form_title"] = "Customs Tariff",
                ["items_form_nav"] = "Customs Tariff - {0}/{1}",
                ["port_classification"] = "Port Classification:",
                ["port_name"] = "Port Name:",
                ["all"] = "-- All --",
                ["internal"] = "Internal (North)",
                ["external"] = "External (South)",
                ["status_select_classification"] = "📌 Select classification then click 'Load Items'",
                ["status_no_data"] = "⚠️ No data for this classification",
                ["status_loaded"] = "✅ Loaded {0} items",
                ["status_loaded_items"] = "📊 Loaded {0} priced + {1} new for {2}",
                ["msg_select_classification"] = "⚠️ Please select port classification first",
                ["msg_no_items"] = "❌ No items in Items Setup screen",
                ["msg_items_loaded"] = "✅ Loaded {0} items\n📌 Priced: {1}\n🆕 New: {2}",
                ["msg_new_items"] = "✅ Loaded {0} new items",
                ["msg_saved"] = "✅ Saved {0} items successfully",
                ["msg_select_row"] = "⚠️ Please select a row first",
                ["msg_no_data_search"] = "⚠️ No data to search",
                ["msg_enter_product_name"] = "Enter product name to search:",
                ["msg_found"] = "✅ Found: {0}",
                ["msg_not_found"] = "❌ Product not found",
                ["msg_no_data_export"] = "No data to export",
                ["msg_exported"] = "✅ Exported {0} records successfully",
                ["col_id"] = "#",
                ["col_product_name"] = "Product Name",
                ["col_weight"] = "Weight",
                ["col_customs_price_usd"] = "Customs Price (USD)",
                ["col_exchange_rate"] = "Exchange Rate",
                ["col_customs_price_ry"] = "Customs Price (RY)",
                ["col_customs_rate_pct"] = "Customs Rate %",
                ["col_customs_value"] = "Customs Value",
                ["col_st_pct"] = "ST %",
                ["col_vat_pct"] = "VAT %",
                ["col_pt_pct"] = "PT %",
                ["col_vf_pct"] = "VF %",
                ["col_tef_pct"] = "TEF %",
                ["col_mff_pct"] = "MFF %",
                ["col_total_customs"] = "Total Customs",
                ["col_total_st"] = "Total ST",
                ["col_total_vat"] = "Total VAT",
                ["col_total_pt"] = "Total PT",
                ["col_total_vf"] = "Total VF",
                ["col_total_tef"] = "Total TEF",
                ["col_total_mff"] = "Total MFF",
                ["item_form_title"] = "Items Data",
                ["grp_header"] = "Item Data",
                ["lbl_item_image"] = "Item Image:",
                ["lbl_item_type"] = "Item Type:",
                ["lbl_group_no"] = "Group No ⭐:",
                ["lbl_group_name"] = "Group Name ⭐:",
                ["lbl_sub_group_under"] = "Sub Group No:",
                ["lbl_item_code"] = "Item Code ⭐:",
                ["lbl_item_name"] = "Item Name ⭐:",
                ["lbl_branch"] = "Branch:",
                ["lbl_ref_no"] = "Ref No:",
                ["lbl_barcode"] = "Barcode:",
                ["lbl_foreign_name"] = "Foreign Name:",
                ["tab_main"] = "Main Data",
                ["tab_units"] = "Item Units",
                ["tab_other"] = "Other Data",
                ["tab_cost"] = "Cost",
                ["tab_movement"] = "Movement",
                ["tab_analysis"] = "Analysis Data",
                ["tab_prices"] = "Prices",
                ["tab_weight_units"] = "Weight Units",
                ["lbl_item_specs"] = "Item Specs:",
                ["lbl_item_specs_foreign"] = "Foreign Specs:",
                ["lbl_assistant"] = "Assistant:",
                ["lbl_detail_group"] = "Detail Group:",
                ["lbl_class"] = "Class:",
                ["lbl_initial_cost"] = "Initial Cost:",
                ["lbl_default_unit"] = "Default Unit:",
                ["lbl_purchase_invoice"] = "Purchase Invoice No:",
                ["lbl_item_activity"] = "Item Activity:",
                ["lbl_return_period"] = "Return Period:",
                ["lbl_rank"] = "Rank:",
                ["lbl_length"] = "Length:",
                ["lbl_size"] = "Size:",
                ["lbl_width"] = "Width:",
                ["lbl_color"] = "Color:",
                ["lbl_height"] = "Height:",
                ["lbl_season"] = "Season:",
                ["lbl_volume"] = "Volume:",
                ["lbl_material"] = "Material:",
                ["lbl_area"] = "Area:",
                ["lbl_brand"] = "Brand:",
                ["lbl_manufacturer"] = "Manufacturer:",
                ["lbl_mfg_no"] = "Mfg No:",
                ["lbl_country_main"] = "Country of Origin:",
                ["lbl_storage"] = "Storage:",
                ["lbl_open_cost"] = "Opening Cost:",
                ["lbl_stop_date"] = "Stop Date:",
                ["lbl_opening_stock"] = "Opening Stock:",
                ["lbl_stop_reason"] = "Stop Reason:",
                ["lbl_available_qty"] = "Available Qty:",
                ["col_unit_num"] = "#",
                ["col_unit_name"] = "Unit",
                ["col_unit_package"] = "Package",
                ["col_weight_unit"] = "Unit",
                ["col_weight_value"] = "Weight",
                ["col_cost_warehouse"] = "Warehouse",
                ["col_cost_warehouse_name"] = "Warehouse Name",
                ["col_cost_initial"] = "Initial Cost",
                ["col_cost_avg"] = "Avg. Cost",
                ["col_cost_available"] = "Available Qty",
                ["col_cost_status"] = "Status",
                ["chk_allow_free_qty"] = "Allow Free Qty",
                ["chk_has_warranty"] = "Has Warranty",
                ["lbl_warranty_period"] = "Warranty Period:",
                ["lbl_warranty_day"] = "Day",
                ["chk_has_commission"] = "Has Commission",
                ["lbl_commission_type"] = "Commission Type:",
                ["lbl_free_qty_percent"] = "Free Qty %:",
                ["lbl_expiry_duration"] = "Expiry Duration:",
                ["lbl_expiry_day"] = "Day",
                ["lbl_min_cost_percent"] = "Min Cost %:",
                ["lbl_max_cost_percent"] = "Max Cost %:",
                ["chk_is_weighted"] = "Weighted Item",
                ["lbl_scale_name"] = "Scale Name:",
                ["lbl_key"] = "Key:",
                ["lbl_doc_type"] = "Doc Type No:",
                ["lbl_doc_ref_no"] = "Doc Ref No:",
                ["lbl_doc_seq"] = "Doc Seq:",
                ["item_type_goods"] = "Goods",
                ["item_type_service"] = "Service",
                ["item_type_asset"] = "Fixed Asset",
                ["item_activity_active"] = "Active",
                ["item_activity_inactive"] = "Inactive",
                ["item_activity_temp"] = "Temporary",
                ["item_activity_stopped"] = "Stopped",
                ["rank_a"] = "A",
                ["rank_b"] = "B",
                ["rank_c"] = "C",
                ["rank_d"] = "D",
                ["rank_e"] = "E",
                ["season_summer"] = "Summer",
                ["season_winter"] = "Winter",
                ["season_spring"] = "Spring",
                ["season_autumn"] = "Autumn",
                ["season_all"] = "All Seasons",
                ["season_none"] = "Not Specified",
                ["default_unit_piece"] = "Piece",
                ["default_unit_kg"] = "Kg",
                ["default_unit_liter"] = "Liter",
                ["default_unit_carton"] = "Carton",
                ["default_unit_can"] = "Can",
                ["msg_group_required"] = "⚠️ Group number is required",
                ["msg_group_name_required"] = "⚠️ Group name is required",
                ["msg_item_code_required"] = "⚠️ Item code is required",
                ["msg_item_name_required"] = "️ Item name is required",
                ["msg_update_success"] = "✅ Item updated successfully",
                ["msg_add_success"] = "✅ Item added successfully",
                ["msg_delete_confirm"] = "️ Delete:\n{0} - {1}?",
                ["msg_delete_title"] = "Confirm",
                ["msg_delete_success"] = "✅ Deleted",
                ["msg_no_item"] = "⚠️ No item selected",
                ["msg_search"] = "🔍 Search",
                ["msg_search_label"] = "Enter item number or name:",
                ["msg_btn_search"] = "Search",
                ["msg_select_unit"] = "🔍 Select unit (double-click or press Enter)",
                ["msg_add_image"] = "Select item image",
                ["msg_printing"] = "🖨️ Printing...",
                ["msg_exporting"] = "📊 Exporting...",
                ["msg_filtering"] = " Filtering...",
                ["success_title"] = "Success",
                ["search_title"] = "Search",
                ["currencies_form_title"] = "💱 Currencies Setup",
                ["grp_currency_data"] = "📦 Currency Data",
                ["grp_audit_log"] = "📊 Audit Log",
                ["lbl_currency_number"] = "Currency No:",
                ["lbl_currency_name"] = "Currency Name:",
                ["lbl_currency_symbol"] = "Symbol:",
                ["lbl_currency_type"] = "Currency Type:",
                ["lbl_exchange_rate"] = "Exchange Rate:",
                ["lbl_min_rate"] = "Min Rate:",
                ["lbl_max_rate"] = "Max Rate:",
                ["lbl_foreign_name"] = "Foreign Name:",
                ["lbl_subunit"] = "Subunit:",
                ["lbl_decimal_places"] = "Decimal Places:",
                ["radio_local"] = "Local",
                ["radio_foreign"] = "Foreign",
                ["chk_stored"] = "Stored",
                ["col_currency"] = "Currency",
                ["col_action"] = "Action",
                ["col_old_rate"] = "Old Rate",
                ["col_new_rate"] = "New Rate",
                ["col_old_min_rate"] = "Old Min Rate",
                ["col_new_min_rate"] = "New Min Rate",
                ["col_old_max_rate"] = "Old Max Rate",
                ["col_new_max_rate"] = "New Max Rate",
                ["col_user"] = "User",
                ["col_modified_date"] = "Date & Time",
                ["msg_currency_number_required"] = "⚠️ Currency number is required",
                ["msg_currency_name_required"] = "⚠️ Currency name is required",
                ["msg_invalid_rate"] = "️ Invalid exchange rate",
                ["msg_save_success"] = "✅ Saved successfully",
                ["msg_edit_success"] = "✅ Edited successfully",
                ["msg_add_success_curr"] = "✅ Added successfully",
                ["msg_delete_success_curr"] = "✅ Deleted",
                ["msg_select_record"] = "⚠️ Please select a record to edit",
                ["msg_select_currency"] = "⚠️ Please select a currency first",
                ["msg_enter_currency_name"] = "⚠️ Enter currency name to search",
                ["msg_confirm_delete_curr"] = "️ Delete '{0}' permanently?",
                ["msg_currency_not_found"] = "⚠️ Currency not found",
                ["msg_not_found_curr"] = "❌ Not found: {0}",
                ["action_add"] = "Add",
                ["action_edit"] = "Edit",
                ["action_delete"] = "Delete",
                ["type_local"] = "Local",
                ["type_foreign"] = "Foreign",
                ["print_title"] = "Currency Audit Report - Yemen Customs System",
                ["print_date"] = "Print Date: {0}",
                ["ports_form_title"] = "️ Ports Setup - Yemen Customs System",
                ["grp_port_data"] = "🏛️ Port Data",
                ["grp_classification"] = "️ Classification",
                ["grp_nature"] = "🌍 Nature",
                ["grp_rate"] = "💰 Customs Exchange Rate",
                ["grp_payment"] = "💳 Payment Method",
                ["lbl_port_number"] = "Port No:",
                ["lbl_port_name"] = "Port Name:",
                ["lbl_port_address"] = "Port Address:",
                ["lbl_port_type"] = "Port Type:",
                ["lbl_currency"] = "Currency:",
                ["radio_internal"] = "Internal (North)",
                ["radio_external"] = "External (South)",
                ["radio_land"] = "Land",
                ["radio_air"] = "Air",
                ["radio_sea"] = "Sea",
                ["radio_rate1"] = "250.5 Rial/USD",
                ["radio_rate2_north"] = "535 Rial/USD",
                ["radio_rate2_south"] = "750 Rial/USD",
                ["radio_cash"] = "Cash",
                ["radio_check"] = "Check",
                ["port_type_main"] = "Main",
                ["port_type_supervisory"] = "Supervisory",
                ["currency_yer"] = "Yemeni Rial",
                ["currency_yer_south"] = "Southern Rial",
                ["rate_info_north"] = "💰 Customs Rate: 250.5 or 535 Yemeni Rial (North)",
                ["rate_info_south"] = "💰 Customs Rate: 250.5 or 750 Southern Rial (South)",
                ["col_port_name"] = "Port Name",
                ["col_port_nature"] = "Port Nature",
                ["col_port_type"] = "Port Type",
                ["col_classification"] = "Classification",
                ["col_exchange_rate_type"] = "Customs Exchange Rate",
                ["col_operation_type"] = "Operation Type",
                ["msg_port_name_required"] = "⚠️ Port name is required",
                ["msg_select_port_edit"] = "Please select a port to edit",
                ["msg_select_port_delete"] = "Please select a port to delete",
                ["msg_confirm_delete_port"] = "🗑️ Are you sure you want to delete '{0}' permanently?",
                ["msg_save_success_port"] = "✅ Port data saved successfully",
                ["msg_edit_success_port"] = "✅ Port data edited successfully",
                ["msg_delete_success_port"] = "✅ Port deleted successfully",
                ["msg_no_ports"] = "No ports to navigate",
                ["msg_first_port"] = "⚠️ You are at the first port",
                ["msg_last_port"] = "⚠️ You are at the last port",
                ["msg_enter_port_name"] = "Please enter port name to search",
                ["msg_found_port"] = "✅ Found: {0}",
                ["msg_not_found_port"] = "❌ No port found containing: {0}",
                ["msg_error_loading"] = "Error loading data: {0}",
                ["search_result"] = "Search Result",
                ["warehouses_form_title"] = "🏢 Warehouses Setup - Yemen Customs System",
                ["grp_warehouse_data"] = "📦 Warehouse Data",
                ["lbl_warehouse_code"] = "Warehouse Code:",
                ["lbl_warehouse_name"] = "Warehouse Name:",
                ["lbl_warehouse_type"] = "Warehouse Type:",
                ["lbl_port_link"] = "Linked Port:",
                ["lbl_address"] = "Address:",
                ["lbl_manager"] = "Manager:",
                ["lbl_phone"] = "Phone Number:",
                ["lbl_status"] = "Status:",
                ["lbl_notes"] = "Notes:",
                ["not_linked"] = "-- Not Linked --",
                ["type_main"] = "Main",
                ["type_sub"] = "Sub",
                ["type_temp"] = "Temporary",
                ["status_active"] = "Active",
                ["status_inactive"] = "Inactive",
                ["col_warehouse_code"] = "Warehouse Code",
                ["col_warehouse_name"] = "Warehouse Name",
                ["col_type"] = "Type",
                ["col_port_link"] = "Linked Port",
                ["col_address"] = "Address",
                ["col_manager"] = "Manager",
                ["col_phone"] = "Phone",
                ["col_status"] = "Status",
                ["msg_warehouse_code_required"] = "️ Warehouse code is required",
                ["msg_warehouse_name_required"] = "⚠️ Warehouse name is required",
                ["msg_select_warehouse_edit"] = "⚠️ Please select a warehouse to edit first",
                ["msg_select_warehouse_delete"] = "⚠️ Please select a warehouse to delete first",
                ["msg_confirm_delete_warehouse"] = "🗑️ Are you sure you want to delete warehouse '{0}'?",
                ["msg_delete_title_wh"] = "Confirm Delete",
                ["msg_edit_success_wh"] = "✏️ Warehouse edited successfully",
                ["msg_add_success_wh"] = "✅ Warehouse added successfully",
                ["msg_delete_success_wh"] = "✅ Warehouse deleted successfully",
                ["msg_not_found_wh"] = "❌ Warehouse not found",
                ["msg_found_wh"] = "✅ Found warehouse: {0}",
                ["msg_enter_search"] = "Enter warehouse name or code to search:",
                ["search_title_wh"] = "Search",
                ["edit_title"] = "Edit",
                ["add_title"] = "Add",
                ["delete_title"] = "Delete",
                ["export_title"] = "Export",
                ["clearance_form_title"] = "📋 Customs Clearance - Yemen Customs System",
                ["lbl_request_number"] = "Request Number:",
                ["lbl_statement_date"] = "Statement Date:",
                ["lbl_statement_no"] = "Statement No:",
                ["lbl_client_name"] = "Client/Branch Name:",
                ["lbl_car_no"] = "Car No:",
                ["lbl_total_fees"] = "💰 Total Customs Fees & Taxes:",
                ["col_quantity"] = "Received Qty",
                ["col_transferred_qty"] = "Transferred Qty",
                ["col_remaining_qty"] = "Remaining Qty",
                ["col_stock_status"] = "Stock Status",
                ["col_weight_total"] = "Total Weight",
                ["col_customs_fee"] = "Customs Fee",
                ["col_customs_fee_total"] = "Total Customs Fee",
                ["col_st"] = "Sales Tax ST",
                ["col_st_total"] = "Total ST",
                ["col_vat"] = "VAT",
                ["col_vat_total"] = "Total VAT",
                ["col_pt"] = "Profit Tax PT",
                ["col_pt_total"] = "Total PT",
                ["col_vf"] = "Improvement VF",
                ["col_vf_total"] = "Total VF",
                ["col_tef"] = "Teacher TEF",
                ["col_tef_total"] = "Total TEF",
                ["col_mff"] = "Martyr MFF",
                ["col_mff_total"] = "Total MFF",
                ["col_other"] = "Other",
                ["col_total"] = "Final Total",
                ["status_at_port"] = "📍 At Port",
                ["status_transferred_full"] = "✅ Fully Transferred",
                ["status_transferred_partial"] = "🚚 Partially Transferred",
                ["msg_enter_statement_no"] = "⚠️ Please enter statement number",
                ["msg_no_data_for_statement"] = "❌ No data for statement: {0}",
                ["msg_statement_loaded"] = "✅ Statement {0} loaded successfully",
                ["msg_enter_statement_first"] = "⚠️ Please enter or select statement number first",
                ["msg_no_movements"] = "❌ No transfer movements for statement: {0}",
                ["movements_form_title"] = "📜 Goods Movements Log - Statement: {0}",
                ["col_date"] = "Transfer Date",
                ["col_product"] = "Product Name",
                ["col_qty"] = "Transferred Qty",
                ["col_from"] = "From",
                ["col_to"] = "To",
                ["col_transfer_no"] = "Transfer No",
                ["col_notes"] = "Notes",
                ["msg_no_clearance_data"] = "📭 No clearance data stored",
                ["statements_list_title"] = "📋 Customs Clearance Data List - Select Statement",
                ["col_items_count"] = "Items Count",
                ["col_total_qty"] = "Total Qty",
                ["col_total_value"] = "Final Total",
                ["msg_select_port_first"] = "️ Please select a port first",
                ["msg_no_items_for_port"] = "❌ No items registered for {0} ports",
                ["items_selection_title"] = "🔍 Select Item - {0}",
                ["north_region"] = "North Region",
                ["south_region"] = "South Region",
                ["msg_item_added"] = "✅ Item added: {0}",
                ["msg_statement_no_required"] = "⚠️ Statement number is required",
                ["msg_records_saved"] = "✅ {0} records saved successfully",
                ["msg_no_statement_to_delete"] = "⚠️ No statement selected to delete",
                ["msg_confirm_delete_statement"] = "🗑️ Are you sure you want to delete statement {0}?",
                ["msg_statement_deleted"] = "✅ Statement deleted successfully",
                ["msg_enter_statement_search"] = "Enter statement number to search:",
                ["msg_statement_found"] = "✅ Found statement: {0}",
                ["msg_statement_not_found"] = "❌ Statement not found",
                ["msg_export_success"] = "✅ Data exported successfully",
                ["msg_select_row_edit"] = "⚠️ Please select the row to edit",
                ["msg_select_statement_first"] = "⚠️ Please select a statement first",
                ["msg_statement_selected"] = "✅ Statement loaded: {0}",
                ["print_system_title"] = "Yemen Customs Integrated System",
                ["print_movements_title"] = "📜 Goods Movements Log - Statement: {0}",
                ["print_port_name"] = "Port Name: {0}",
                ["print_request_number"] = "Request Number: {0}",
                ["print_statement_date"] = "Statement Date: {0}",
                ["print_client_name"] = "Client/Branch: {0}",
                ["print_car_no"] = "Car No: {0}",
                ["print_total_transferred"] = "Total Transferred Qty: {0}",
                ["print_movements_count"] = "Movements Count: {0}",
                ["exchange_rate_selection_title"] = "💱 Select Customs Exchange Rate",
                ["additional_amounts_form_title"] = "💰 Additional Amounts in Statement",
                ["lbl_fee_no"] = "Additional Fee No:",
                ["lbl_fee_name"] = "Additional Fee Name:",
                ["lbl_effect_type"] = "Effect Type:",
                ["effect_item"] = "Item Level",
                ["effect_statement"] = "Statement Level",
                ["effect_truck"] = "Truck Level",
                ["fee_by_port"] = "By Port",
                ["fee_by_port_group"] = "By Port Group"
            };

            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                ["ar"] = arabic,
                ["en"] = english
            };
        }

        private static void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null && !string.IsNullOrEmpty(settings.Language))
                    {
                        _currentLanguage = settings.Language;
                    }
                }
            }
            catch { }
        }

        private static void SaveSettings()
        {
            try
            {
                var settings = new AppSettings { Language = _currentLanguage };
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);
            }
            catch { }
        }

        public static string Get(string key)
        {
            if (_translations == null) LoadTranslations();
            if (_translations.TryGetValue(_currentLanguage, out var lang))
            {
                if (lang.TryGetValue(key, out var translation))
                    return translation;
            }
            return key;
        }

        public static void ApplyLanguage(Form form)
        {
            if (form == null) return;
            form.RightToLeft = RightToLeft;
            form.RightToLeftLayout = IsRTL;
            ApplyLanguageToControls(form.Controls);
        }

        private static void ApplyLanguageToControls(Control.ControlCollection controls)
        {
            if (controls == null) return;
            foreach (Control control in controls)
            {
                ApplyLanguageToControl(control);
                if (control.Controls.Count > 0)
                    ApplyLanguageToControls(control.Controls);
            }
        }

        private static void ApplyLanguageToControl(Control control)
        {
            if (control == null) return;

            if (control is ToolStrip toolStrip)
            {
                foreach (ToolStripItem item in toolStrip.Items)
                    ApplyLanguageToToolStripItem(item);
            }

            if (control is MenuStrip menuStrip)
            {
                foreach (ToolStripMenuItem item in menuStrip.Items)
                    ApplyLanguageToMenuItem(item);
            }

            if (!string.IsNullOrEmpty(control.Tag?.ToString()))
            {
                string key = control.Tag.ToString();
                string translated = Get(key);
                if (translated != key && !string.IsNullOrEmpty(translated))
                {
                    switch (control)
                    {
                        case Button btn: btn.Text = translated; break;
                        case Label lbl: lbl.Text = translated; break;
                        case CheckBox chk: chk.Text = translated; break;
                        case TabPage tp: tp.Text = translated; break;
                        case GroupBox gb: gb.Text = translated; break;
                    }
                }
            }
        }

        private static void ApplyLanguageToToolStripItem(ToolStripItem item)
        {
            if (item == null) return;
            if (!string.IsNullOrEmpty(item.Tag?.ToString()))
            {
                string key = item.Tag.ToString();
                string translated = Get(key);
                if (translated != key && !string.IsNullOrEmpty(translated))
                    item.Text = translated;
            }
            if (item is ToolStripMenuItem menuItem)
            {
                foreach (ToolStripItem child in menuItem.DropDownItems)
                    ApplyLanguageToToolStripItem(child);
            }
        }

        private static void ApplyLanguageToMenuItem(ToolStripMenuItem item)
        {
            if (item == null) return;
            if (!string.IsNullOrEmpty(item.Tag?.ToString()))
            {
                string key = item.Tag.ToString();
                string translated = Get(key);
                if (translated != key && !string.IsNullOrEmpty(translated))
                    item.Text = translated;
            }
            foreach (ToolStripItem child in item.DropDownItems)
            {
                if (child is ToolStripMenuItem childMenu)
                    ApplyLanguageToMenuItem(childMenu);
            }
        }

        private class AppSettings
        {
            public string Language { get; set; }
        }
    }

    // ==================== ✅ النموذج الرئيسي ====================
    public partial class MainForm : Form
    {
        private ToolStripMenuItem mnuLanguage;
        private ToolStripMenuItem mnuArabic;
        private ToolStripMenuItem mnuEnglish;

        private UserManager.User currentUser;
        private ToolStrip toolStripTop;
        private ToolStripLabel lblUserInfo;
        private ToolStripButton btnLogout;

        public MainForm(UserManager.User user)
        {
            DatabaseHelper.EnsureAllTablesExist();
            Application.AddMessageFilter(new SecurityMessageFilter());

            LanguageManager.Initialize();
            InitializeComponent();
            DatabaseHelper.TestConnection();

            currentUser = user;
            SetupTopToolBar();
            SetupUserInfo();
            ApplyUserPermissions();

            SetupTreeMenu();
            SetupLanguageMenu();
            ApplyCurrentLanguage();
            ShowWelcome();

            LanguageManager.LanguageChanged += (s, e) => ApplyCurrentLanguage();
        }

        private void SetupTopToolBar()
        {
            toolStripTop = new ToolStrip();
            toolStripTop.Dock = DockStyle.Top;
            toolStripTop.BackColor = Color.FromArgb(44, 62, 80);
            toolStripTop.Height = 40;
            toolStripTop.GripStyle = ToolStripGripStyle.Hidden;
            toolStripTop.RightToLeft = RightToLeft.Yes;

            btnLogout = new ToolStripButton();
            btnLogout.Text = LanguageManager.Get("btn_logout");
            btnLogout.ForeColor = Color.White;
            btnLogout.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnLogout.BackColor = Color.FromArgb(231, 76, 60);
            btnLogout.Click += BtnLogout_Click;

            lblUserInfo = new ToolStripLabel();
            lblUserInfo.ForeColor = Color.White;
            lblUserInfo.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblUserInfo.Text = "👤 المستخدم";

            toolStripTop.Items.Add(btnLogout);
            toolStripTop.Items.Add(new ToolStripSeparator());
            toolStripTop.Items.Add(lblUserInfo);

            this.Controls.Add(toolStripTop);
        }

        private void SetupUserInfo()
        {
            if (currentUser != null && lblUserInfo != null)
            {
                string roleText = currentUser.Role == "Admin" ? "👑 مدير" : "👤 مخلص";
                lblUserInfo.Text = $"👤 {currentUser.FullName} | {roleText}";
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LanguageManager.Get("msg_confirm_logout"),
                LanguageManager.Get("msg_confirm"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserManager.Logout();
                this.Close();
                Application.Restart();
            }
        }

        private void ApplyUserPermissions()
        {
            if (currentUser == null) return;

            foreach (TreeNode node in treeMenu.Nodes)
            {
                ApplyPermissionsToNode(node);
            }

            if (!UserManager.IsAdmin() && !UserManager.HasPermission(UserManager.PERM_USERS))
            {
                RemoveUsersNode();
            }
        }

        private void ApplyPermissionsToNode(TreeNode node)
        {
            string tag = node.Tag?.ToString() ?? "";
            bool hasPermission = true;

            switch (tag)
            {
                case "Currencies": hasPermission = UserManager.HasPermission(UserManager.PERM_PORTS); break;
                case "Ports": hasPermission = UserManager.HasPermission(UserManager.PERM_PORTS); break;
                case "Warehouses": hasPermission = UserManager.HasPermission(UserManager.PERM_WAREHOUSES); break;
                case "Items": hasPermission = UserManager.HasPermission(UserManager.PERM_ITEMS); break;
                case "Groups":
                case "SubGroups":
                    hasPermission = UserManager.HasPermission(UserManager.PERM_ITEMS);
                    break;
                case "ReviewTariffRequests":
                    hasPermission = UserManager.IsAdmin();
                    break;
                case "TariffRequest":
                    hasPermission = UserManager.HasPermission(UserManager.PERM_CLEARANCE);
                    break;
                case "ItemsData": hasPermission = UserManager.HasPermission(UserManager.PERM_ITEMS_DATA); break;
                case "CustomsClearance": hasPermission = UserManager.HasPermission(UserManager.PERM_CLEARANCE); break;
                case "StockTransfer": hasPermission = UserManager.HasPermission(UserManager.PERM_STOCK_TRANSFER); break;
                case "WarehouseTransfer": hasPermission = UserManager.HasPermission(UserManager.PERM_WAREHOUSE_TRANSFER); break;
                case "Users": hasPermission = UserManager.HasPermission(UserManager.PERM_USERS); break;
                case "CurrencyReports": hasPermission = UserManager.HasPermission(UserManager.PERM_REPORTS); break;
                case "ClearanceReports": hasPermission = UserManager.HasPermission(UserManager.PERM_REPORTS); break;
                case "WarehouseReports": hasPermission = UserManager.HasPermission(UserManager.PERM_REPORTS); break;
            }

            if (!hasPermission)
            {
                node.ForeColor = Color.Gray;
                node.NodeFont = new Font(treeMenu.Font, FontStyle.Strikeout);
            }
            else
            {
                node.ForeColor = treeMenu.ForeColor;
                node.NodeFont = treeMenu.Font;
            }

            foreach (TreeNode child in node.Nodes)
            {
                ApplyPermissionsToNode(child);
            }
        }

        private void RemoveUsersNode()
        {
            foreach (TreeNode parent in treeMenu.Nodes)
            {
                List<TreeNode> toRemove = new List<TreeNode>();
                foreach (TreeNode child in parent.Nodes)
                {
                    if (child.Tag?.ToString() == "Users")
                        toRemove.Add(child);
                }
                foreach (TreeNode node in toRemove)
                    parent.Nodes.Remove(node);
            }
        }

        private string GetRequiredPermission(string formTag)
        {
            switch (formTag)
            {
                case "Currencies":
                case "Ports":
                    return UserManager.PERM_PORTS;
                case "Warehouses":
                    return UserManager.PERM_WAREHOUSES;
                case "Items":
                    return UserManager.PERM_ITEMS;
                case "ReviewTariffRequests":
                    return UserManager.PERM_USERS;
                case "TariffRequest":
                    return UserManager.PERM_CLEARANCE;
                case "Groups":
                case "SubGroups":
                    return UserManager.PERM_ITEMS;
                case "ItemsData":
                case "CustomsPricing":
                    return UserManager.PERM_ITEMS_DATA;
                case "CustomersBranches":
                    return UserManager.PERM_ITEMS;
                case "CustomsClearance":
                    return UserManager.PERM_CLEARANCE;
                case "StockTransfer":
                case "PortToWarehouseTransfer":
                    return UserManager.PERM_STOCK_TRANSFER;
                case "WarehouseTransfer":
                case "WarehouseToWarehouseTransfer":
                    return UserManager.PERM_WAREHOUSE_TRANSFER;
                case "Users":
                case "UsersData":
                case "ScreensPermissions":
                case "InputsPermissions":
                case "ChangePassword":
                    return UserManager.PERM_USERS;
                case "CurrencyReports":
                case "ClearanceReports":
                case "WarehouseReports":
                case "ClearanceRequestsReports":
                case "FeesAdditionalAmountsReports":
                case "PermitsReleasesUnitReports":
                case "PermitsReleasesMovementReports":
                    return UserManager.PERM_REPORTS;
                default:
                    return "";
            }
        }

        private void SetupLanguageMenu()
        {
            mnuLanguage = new ToolStripMenuItem();
            mnuLanguage.Text = LanguageManager.Get("language_menu");
            mnuLanguage.Name = "mnuLanguage";

            mnuArabic = new ToolStripMenuItem();
            mnuArabic.Text = LanguageManager.Get("lang_arabic");
            mnuArabic.Name = "mnuArabic";
            mnuArabic.Click += (s, e) => LanguageManager.CurrentLanguage = "ar";

            mnuEnglish = new ToolStripMenuItem();
            mnuEnglish.Text = LanguageManager.Get("lang_english");
            mnuEnglish.Name = "mnuEnglish";
            mnuEnglish.Click += (s, e) => LanguageManager.CurrentLanguage = "en";

            mnuLanguage.DropDownItems.Add(mnuArabic);
            mnuLanguage.DropDownItems.Add(mnuEnglish);

            menuStrip1.Items.Insert(0, mnuLanguage);
        }

        private void ApplyCurrentLanguage()
        {
            mnuArabic.Checked = LanguageManager.CurrentLanguage == "ar";
            mnuEnglish.Checked = LanguageManager.CurrentLanguage == "en";

            mnuLanguage.Text = LanguageManager.Get("language_menu");
            mnuArabic.Text = LanguageManager.Get("lang_arabic");
            mnuEnglish.Text = LanguageManager.Get("lang_english");

            if (btnLogout != null) btnLogout.Text = LanguageManager.Get("btn_logout");

            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;

            if (lblWelcome != null) lblWelcome.Text = LanguageManager.Get("welcome_title");

            SetupTreeMenu();
        }

        public void ShowWelcome()
        {
            foreach (Form child in this.MdiChildren.ToList())
            {
                if (!child.IsDisposed) child.Close();
            }
            if (lblWelcome != null) { lblWelcome.Visible = true; lblWelcome.BringToFront(); }
            if (pnlMenu != null) { pnlMenu.Visible = true; pnlMenu.BringToFront(); }
            if (treeMenu != null) { treeMenu.Visible = true; }
            this.Refresh();
            this.Update();
            this.Activate();
            this.BringToFront();
        }

        public void HideWelcome()
        {
            if (lblWelcome != null) lblWelcome.Visible = false;
            if (pnlMenu != null) pnlMenu.Visible = false;
        }

        private void SetupTreeMenu()
        {
            if (treeMenu == null) return;
            treeMenu.Nodes.Clear();

            TreeNode nodeSystemSetup = new TreeNode(" تهيئة النظام");
            nodeSystemSetup.Nodes.Add(new TreeNode("تهيئة العملات") { Tag = "Currencies" });
            nodeSystemSetup.Nodes.Add(new TreeNode("مستويات الاعتماد") { Tag = "ApprovalLevels" });

            TreeNode nodeSystemManagement = new TreeNode(" إدارة النظام");
            TreeNode nodeUsers = new TreeNode("المستخدمين");
            nodeUsers.Nodes.Add(new TreeNode("بيانات المستخدمين") { Tag = "UsersData" });
            nodeUsers.Nodes.Add(new TreeNode("صلاحيات الشاشات") { Tag = "ScreensPermissions" });
            nodeUsers.Nodes.Add(new TreeNode("صلاحيات المدخلات") { Tag = "InputsPermissions" });
            nodeUsers.Nodes.Add(new TreeNode("تغيير كلمة السر") { Tag = "ChangePassword" });
            nodeUsers.Nodes.Add(new TreeNode("إضافة مستخدم جديد") { Tag = "AddUser" });

            TreeNode nodeBackup = new TreeNode("الأقفال والنسخ الاحتياطي");
            nodeBackup.Nodes.Add(new TreeNode("الأقفال الشهري") { Tag = "MonthlyLocks" });
            nodeBackup.Nodes.Add(new TreeNode("الأقفال السنوي") { Tag = "AnnualLocks" });
            nodeBackup.Nodes.Add(new TreeNode("اعدادات النسخ الاحتياطي") { Tag = "BackupSettings" });
            nodeBackup.Nodes.Add(new TreeNode("استرجاع النسخه الأحتياطيه") { Tag = "RestoreBackup" });

            nodeSystemManagement.Nodes.Add(nodeUsers);
            nodeSystemManagement.Nodes.Add(nodeBackup);

            TreeNode nodeWarehouses = new TreeNode(" أنظمة المخازن");
            nodeWarehouses.Nodes.Add(new TreeNode(" وحدات القياس") { Tag = "Units" });
            nodeWarehouses.Nodes.Add(new TreeNode(" وحدات الأوزان") { Tag = "WeightUnits" });
            nodeWarehouses.Nodes.Add(new TreeNode(" بيانات المخازن") { Tag = "Warehouses" });
            nodeWarehouses.Nodes.Add(new TreeNode("  المجموعة الرئيسيه للأصناف") { Tag = "ItemsGroup" });
            nodeWarehouses.Nodes.Add(new TreeNode(" المجموعات الفرعية") { Tag = "SubGroups" });
            nodeWarehouses.Nodes.Add(new TreeNode(" بيانات  الأصناف") { Tag = "Items" });

            TreeNode nodeTransferTypes = new TreeNode(" أنواع التحويل المخزني");
            nodeTransferTypes.Nodes.Add(new TreeNode(" تحويل من منفذ إلى مخزن") { Tag = "PortToWarehouseTransfer" });
            nodeTransferTypes.Nodes.Add(new TreeNode(" تحويل من مخزن إلى مخزن") { Tag = "WarehouseToWarehouseTransfer" });
            nodeWarehouses.Nodes.Add(nodeTransferTypes);

            nodeWarehouses.Nodes.Add(new TreeNode(" استلام التحويل المخزني") { Tag = "ReceiveWarehouseTransfer" });
            nodeWarehouses.Nodes.Add(new TreeNode(" تسوية مخزون") { Tag = "InventorySettlement" });
            nodeWarehouses.Nodes.Add(new TreeNode(" تقارير التحويل والاستلامات المخزنية") { Tag = "TransferReceiptsReports" });
            nodeWarehouses.Nodes.Add(new TreeNode(" تقارير تسوية مخزون") { Tag = "InventorySettlementReports" });

            TreeNode nodeCustomsSystem = new TreeNode(" نظام التخليص الجمركي");
            TreeNode nodeSetup = new TreeNode("التهيئة");
            nodeSetup.Nodes.Add(new TreeNode(" أنواع الترسيم الجمركي") { Tag = "CustomsTariffTypes" });
            nodeSetup.Nodes.Add(new TreeNode(" التسعيرة الجمركية") { Tag = "CustomsPricing" });
            nodeSetup.Nodes.Add(new TreeNode(" مجموعة المنافذ") { Tag = "PortsGroup" });
            nodeSetup.Nodes.Add(new TreeNode(" بيانات المنافذ") { Tag = "PortsData" });
            nodeSetup.Nodes.Add(new TreeNode(" أنواع المبالغ الإضافية في البيان") { Tag = "AdditionalAmountsTypes" });
            nodeSetup.Nodes.Add(new TreeNode(" المبالغ الإضافية في البيان") { Tag = "AdditionalAmounts" });
            nodeSetup.Nodes.Add(new TreeNode(" بيانات العملاء والفروع") { Tag = "CustomersBranches" });

            TreeNode nodeOperations = new TreeNode(" العمليات");
           
            nodeOperations.Nodes.Add(new TreeNode("طلب ترسيم") { Tag = "TariffRequest" });
            nodeOperations.Nodes.Add(new TreeNode(" تخليص جمركي") { Tag = "CustomsClearance" });
            nodeOperations.Nodes.Add(new TreeNode("مراجعة طلبات الترسيم") { Tag = "ReviewTariffRequests" });
            nodeOperations.Nodes.Add(new TreeNode(" التصاريح الزراعية") { Tag = "AgriculturalPermits" });
            nodeOperations.Nodes.Add(new TreeNode(" إفراج المدفوعات") { Tag = "PaymentsRelease" });
            nodeOperations.Nodes.Add(new TreeNode(" تسوية التصاريح والمدفوعات") { Tag = "PermitsPaymentsSettlement" });

            nodeCustomsSystem.Nodes.Add(nodeSetup);
            nodeCustomsSystem.Nodes.Add(nodeOperations);

            TreeNode nodeReports = new TreeNode(" التقارير");
            nodeReports.Nodes.Add(new TreeNode(" تقارير طلبات التخليص") { Tag = "ClearanceRequestsReports" });
            nodeReports.Nodes.Add(new TreeNode(" تقارير التخليص") { Tag = "ClearanceReports" });
            nodeReports.Nodes.Add(new TreeNode(" تقارير الرسوم والمبالغ الإضافية للبيانات") { Tag = "FeesAdditionalAmountsReports" });
            nodeReports.Nodes.Add(new TreeNode(" تقارير وحدة التصاريح والإفراجات") { Tag = "PermitsReleasesUnitReports" });
            nodeReports.Nodes.Add(new TreeNode(" تقارير حركة التصاريح والإفراجات") { Tag = "PermitsReleasesMovementReports" });

            nodeSystemSetup.Expand();
            nodeSystemManagement.Expand();
            nodeWarehouses.Expand();
            nodeCustomsSystem.Expand();
            nodeSetup.Expand();
            nodeOperations.Expand();
            nodeReports.Expand();

            treeMenu.Nodes.AddRange(new TreeNode[] {
                nodeSystemSetup,
                nodeSystemManagement,
                nodeWarehouses,
                nodeCustomsSystem,
                nodeReports
            });

            ApplyScreenPermissions();
        }

        private void ApplyScreenPermissions()
        {
            if (currentUser == null) return;

            UserManager.LoadScreenPermissionsFromDatabase(currentUser.Id);
            var userPermissions = UserManager.GetScreenPermissions(currentUser.Id);

            if (currentUser.Role == "Admin" || userPermissions.Count == 0)
            {
                List<ScreenPermission> allPermissions = new List<ScreenPermission>();
                foreach (var screen in UserManager.GetAllSystemScreens())
                {
                    screen.Include = true;
                    screen.Print = true;
                    screen.ViewReport = true;
                    screen.AddFrom = true;
                    screen.Add = true;
                    screen.Edit = true;
                    screen.Delete = true;
                    screen.View = true;
                    screen.ViewList = true;
                    screen.SendTo = true;
                    screen.Review = true;
                    screen.Post = true;
                    screen.CancelPost = true;
                    allPermissions.Add(screen);
                }
                UserManager.SaveScreenPermissions(currentUser.Id, allPermissions);
                userPermissions = allPermissions;
            }

            foreach (TreeNode node in treeMenu.Nodes)
            {
                ApplyPermissionsToNode(node, userPermissions);
            }
        }

        private void mnuAddUser_Click(object sender, EventArgs e)
        {
            if (!UserManager.IsAdmin())
            {
                MessageBox.Show("🚫 هذه الشاشة متاحة للمدير فقط", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var addUserForm = new AddUserForm())
            {
                if (addUserForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("✅ تم إضافة المستخدم بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ApplyPermissionsToNode(TreeNode node, List<ScreenPermission> userPermissions)
        {
            string tag = node.Tag?.ToString();
            if (!string.IsNullOrEmpty(tag))
            {
                var screenPerm = userPermissions.FirstOrDefault(p => p.ScreenTag == tag);
                if (userPermissions == null || userPermissions.Count == 0) { }
                else if (screenPerm == null || !screenPerm.IsGranted())
                {
                    node.Remove();
                    return;
                }
            }
            List<TreeNode> childNodes = new List<TreeNode>();
            foreach (TreeNode child in node.Nodes) { childNodes.Add(child); }
            foreach (TreeNode child in childNodes) { ApplyPermissionsToNode(child, userPermissions); }
        }

        private void treeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeMenu.SelectedNode != null && treeMenu.SelectedNode.Tag != null)
            {
                string formTag = treeMenu.SelectedNode.Tag.ToString();
                OpenFormByTag(formTag);
                treeMenu.SelectedNode = null;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            HideWelcome();
            foreach (Form existingForm in this.MdiChildren)
            {
                if (existingForm.GetType() == childForm.GetType())
                {
                    existingForm.Activate();
                    existingForm.WindowState = FormWindowState.Maximized;
                    childForm.Dispose();
                    return;
                }
            }
            childForm.MdiParent = this;
            childForm.ControlBox = false;
            childForm.MinimizeBox = false;
            childForm.MaximizeBox = false;
            childForm.WindowState = FormWindowState.Maximized;
            childForm.Dock = DockStyle.Fill;
            childForm.TopLevel = false;
            LanguageManager.ApplyLanguage(childForm);
            childForm.Show();
            childForm.BringToFront();
            childForm.Focus();
        }

        private void OpenFormByTag(string formTag)
        {
            ScreenSecurity.CurrentTag = formTag;
            Form frm = null;
            switch (formTag)
            {
                case "AddUser":
                    if (!UserManager.IsAdmin())
                    {
                        MessageBox.Show("🚫 هذه الشاشة متاحة للمدير فقط", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    using (var addForm = new AddUserForm()) { addForm.ShowDialog(this); }
                    return;
                case "Currencies":
                    frm = new CurrenciesForm();
                    break;
                case "TariffRequest":
                    frm = new TariffRequestForm();
                    break;
                case "ReviewTariffRequests":
                    if (!UserManager.IsAdmin()) { MessageBox.Show("🚫 هذه الشاشة للمدير فقط", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    frm = new ReviewTariffRequestsForm();
                    break;
                case "ApprovalLevels":
                    if (!UserManager.IsAdmin())
                    {
                        MessageBox.Show("🚫 هذه الشاشة متاحة للمدير فقط", LanguageManager.Get("msg_warning"),
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    frm = new ApprovalLevelsForm();
                    break;
                case "UsersData":
                case "ScreensPermissions":
                    if (UserManager.IsAdmin())
                        frm = new ScreenPermissionsForm();
                    else
                    {
                        MessageBox.Show(" هذه الشاشة متاحة للمدير فقط", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    break;
                case "InputsPermissions":
                    frm = new InputPermissionsForm();
                    break;
                case "ChangePassword":
                    if (UserManager.IsAdmin())
                        frm = new UsersManagementForm();
                    else
                    {
                        MessageBox.Show("🚫 هذه الشاشة متاحة للمدير فقط", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    break;
                case "MonthlyLocks":
                case "AnnualLocks":
                case "BackupSettings":
                case "RestoreBackup":
                    MessageBox.Show(formTag + " - " + LanguageManager.Get("msg_in_development"));
                    return;
                case "Units":
                    frm = new UnitsForm();
                    break;
                case "WeightUnits":
                    frm = new WeightUnitsForm();
                    break;
                case "AgriculturalPermits":
                    frm = new AgriculturalPermitsForm();
                    break;
                case "PaymentsRelease":
                    frm = new PaymentsReleaseForm();
                    break;
                case "Warehouses":
                    frm = new WarehousesForm();
                    break;
                case "ItemsGroup":
                    frm = new GroupsForm();
                    break;
                case "SubGroups":
                    frm = new SubGroupsForm();
                    break;
                case "Items":
                    frm = new ItemDataForm();
                    break;
                case "TransferTypes":
                    MessageBox.Show(formTag + " - " + LanguageManager.Get("msg_in_development"));
                    return;
                case "PortToWarehouseTransfer":
                    frm = new StockTransferForm();
                    break;
                case "WarehouseToWarehouseTransfer":
                    frm = new WarehouseToWarehouseTransferForm();
                    break;
                case "ReceiveWarehouseTransfer":
                case "InventorySettlement":
                case "TransferReceiptsReports":
                case "InventorySettlementReports":
                    MessageBox.Show(formTag + " - " + LanguageManager.Get("msg_in_development"));
                    return;
                case "CustomsTariffTypes":
                    CustomsTariffTypesForm.EnsureTableExists();
                    frm = new CustomsTariffTypesForm();
                    break;
                case "CustomsPricing":
                    frm = new ItemsDataForm();
                    break;

                // ✅ التعديل هنا: فصل مجموعة المنافذ عن بيانات المنافذ
                case "PortsGroup":
                    frm = new PortGroupsForm();
                    break;
                case "PortsData":
                    frm = new PortsForm();
                    break;

                case "AdditionalAmountsTypes":
                    MessageBox.Show(formTag + " - " + LanguageManager.Get("msg_in_development"));
                    return;
                case "AdditionalAmounts":
                    frm = new AdditionalAmountsForm();
                    break;
                case "CustomersBranches":
                    frm = new CustomersBranchesForm();
                    break;
                case "ClearanceRequest":
                case "CustomsClearance":
                    frm = new CustomsClearanceForm();
                    break;
                case "ClearanceRequestsReports":
                case "ClearanceReports":
                case "FeesAdditionalAmountsReports":
                case "PermitsReleasesUnitReports":
                case "PermitsReleasesMovementReports":
                    MessageBox.Show(formTag + " - " + LanguageManager.Get("msg_in_development"));
                    return;
                default:
                    MessageBox.Show("⚠️ " + formTag + " - " + LanguageManager.Get("msg_in_development"));
                    return;
            }

            if (frm != null)
            {
                ScreenSecurity.Apply(frm, formTag);
                OpenChildForm(frm);
            }
        }

        private void mnuCurrencies_Click(object sender, EventArgs e) => OpenFormByTag("Currencies");
        private void mnuPorts_Click(object sender, EventArgs e) => OpenFormByTag("Ports");
        private void mnuWarehouses_Click(object sender, EventArgs e) => OpenFormByTag("Warehouses");
        private void mnuItems_Click(object sender, EventArgs e) => OpenFormByTag("Items");
        private void mnuItemsData_Click(object sender, EventArgs e) => OpenFormByTag("ItemsData");
        private void mnuDeclarations_Click(object sender, EventArgs e) => OpenFormByTag("Declarations");
        private void mnuChartOfAccounts_Click(object sender, EventArgs e) => OpenFormByTag("ChartOfAccounts");
        private void mnuNewDeclaration_Click(object sender, EventArgs e) => OpenFormByTag("NewDeclaration");
        private void mnuClearance_Click(object sender, EventArgs e) => OpenFormByTag("ClearanceItems");
        private void mnuCustomsClearance_Click(object sender, EventArgs e) => OpenFormByTag("CustomsClearance");
        private void mnuStockTransfer_Click(object sender, EventArgs e) => OpenFormByTag("StockTransfer");
        private void mnuWarehouseTransfer_Click(object sender, EventArgs e) => OpenFormByTag("WarehouseTransfer");
        private void mnuCurrencyReports_Click(object sender, EventArgs e) => OpenFormByTag("CurrencyReports");
        private void mnuClearanceReports_Click(object sender, EventArgs e) => OpenFormByTag("ClearanceReports");
        private void mnuWarehouseReports_Click(object sender, EventArgs e) => OpenFormByTag("WarehouseReports");
        private void mnuTariffRequest_Click(object sender, EventArgs e) => OpenFormByTag("TariffRequest");
        private void mnuReviewTariffRequests_Click(object sender, EventArgs e) => OpenFormByTag("ReviewTariffRequests");

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LanguageManager.Get("msg_confirm_exit"), LanguageManager.Get("menu_exit"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) ShowWelcome();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;
            ShowWelcome();
            ApplyScreenPermissions();
            CheckPendingTariffRequests();
        }

        private void CheckPendingTariffRequests()
        {
            if (!UserManager.IsAdmin()) return;
            try
            {
                var count = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM TariffRequests WHERE Status = 'pending'");
                int pendingCount = count != null ? Convert.ToInt32(count) : 0;
                if (pendingCount > 0)
                {
                    MessageBox.Show($" لديك {pendingCount} طلب ترسيم جديد في انتظار المراجعة", "إشعار", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }
        }

        private void ApplyScreenPermissionsToTree()
        {
            if (currentUser == null) return;
            int userId = currentUser.Id;
            UserManager.LoadScreenPermissionsFromDatabase(userId);
            var userPermissions = UserManager.GetScreenPermissions(userId);
            foreach (TreeNode node in treeMenu.Nodes)
            {
                ApplyScreenPermissionsToNode(node, userPermissions);
            }
        }

        private void ApplyScreenPermissionsToNode(TreeNode node, List<ScreenPermission> userPermissions)
        {
            string tag = node.Tag?.ToString() ?? "";
            if (!string.IsNullOrEmpty(tag))
            {
                var screenPerm = userPermissions.FirstOrDefault(p => p.ScreenTag == tag);
                if (screenPerm == null || !screenPerm.IsGranted())
                {
                    node.ForeColor = Color.Gray;
                    node.NodeFont = new Font(treeMenu.Font, FontStyle.Strikeout);
                }
                else
                {
                    node.ForeColor = treeMenu.ForeColor;
                    node.NodeFont = treeMenu.Font;
                }
            }
            foreach (TreeNode child in node.Nodes)
            {
                ApplyScreenPermissionsToNode(child, userPermissions);
            }
        }
    }
}