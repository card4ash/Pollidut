﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pollidut.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PollidutEntities : DbContext
    {
        public PollidutEntities()
            : base("name=PollidutEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AREA> AREAS { get; set; }
        public virtual DbSet<BANK> BANKS { get; set; }
        public virtual DbSet<BRAND> BRANDS { get; set; }
        public virtual DbSet<CM_POSITIONS> CM_POSITIONS { get; set; }
        public virtual DbSet<CM> CMS { get; set; }
        public virtual DbSet<ComplainTicket> ComplainTickets { get; set; }
        public virtual DbSet<DESIGNATION> DESIGNATIONS { get; set; }
        public virtual DbSet<DISTRIBUTION_HOUSE_MAIN> DISTRIBUTION_HOUSE_MAIN { get; set; }
        public virtual DbSet<DISTRIBUTION_HOUSES> DISTRIBUTION_HOUSES { get; set; }
        public virtual DbSet<DISTRICT> DISTRICTS { get; set; }
        public virtual DbSet<EDUCATIONAL_DEGREES> EDUCATIONAL_DEGREES { get; set; }
        public virtual DbSet<EDUCATIONAL_DIVISIONS> EDUCATIONAL_DIVISIONS { get; set; }
        public virtual DbSet<EDUCATIONAL_GRADES> EDUCATIONAL_GRADES { get; set; }
        public virtual DbSet<EMPLOYEE_DISTRIBUTION_HOUSES> EMPLOYEE_DISTRIBUTION_HOUSES { get; set; }
        public virtual DbSet<EMPLOYEE_ROLE_STATUSES> EMPLOYEE_ROLE_STATUSES { get; set; }
        public virtual DbSet<EMPLOYEE_TYPES> EMPLOYEE_TYPES { get; set; }
        public virtual DbSet<EMPLOYEE_WORK> EMPLOYEE_WORK { get; set; }
        public virtual DbSet<EMPLOYEE> EMPLOYEES { get; set; }
        public virtual DbSet<FORM_ROW_ITEM> FORM_ROW_ITEM { get; set; }
        public virtual DbSet<GENDER> GENDERS { get; set; }
        public virtual DbSet<JCOffer> JCOffers { get; set; }
        public virtual DbSet<MARITAL_STATUSES> MARITAL_STATUSES { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<PALLYDUT_TARGET> PALLYDUT_TARGET { get; set; }
        public virtual DbSet<PD_PRODUCT_INDENT_FORM> PD_PRODUCT_INDENT_FORM { get; set; }
        public virtual DbSet<PERSON_CURRENT_STATUSES> PERSON_CURRENT_STATUSES { get; set; }
        public virtual DbSet<PERSON_DEGREES> PERSON_DEGREES { get; set; }
        public virtual DbSet<PERSON_FAMILY> PERSON_FAMILY { get; set; }
        public virtual DbSet<PERSON_JOB_EXPERIENCES> PERSON_JOB_EXPERIENCES { get; set; }
        public virtual DbSet<PERSON> PERSONS { get; set; }
        public virtual DbSet<PERSONS_REFERENCES> PERSONS_REFERENCES { get; set; }
        public virtual DbSet<PRODUCT> PRODUCTs { get; set; }
        public virtual DbSet<PRODUCT_SKU> PRODUCT_SKU { get; set; }
        public virtual DbSet<PRODUCT_TYPE> PRODUCT_TYPE { get; set; }
        public virtual DbSet<PROJECT> PROJECTS { get; set; }
        public virtual DbSet<REGION> REGIONS { get; set; }
        public virtual DbSet<RELIGION> RELIGIONS { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<THANA> THANAS { get; set; }
        public virtual DbSet<TICKET_CATEGORY> TICKET_CATEGORY { get; set; }
        public virtual DbSet<TICKET_REPLIED> TICKET_REPLIED { get; set; }
        public virtual DbSet<TICKET_STATUS> TICKET_STATUS { get; set; }
        public virtual DbSet<TICKET_SUB_CATEGORY> TICKET_SUB_CATEGORY { get; set; }
        public virtual DbSet<USER> USERS { get; set; }
        public virtual DbSet<USERS_LOGIN_INFO> USERS_LOGIN_INFO { get; set; }
        public virtual DbSet<DISTRIBUTION_HOUSE_TYPES> DISTRIBUTION_HOUSE_TYPES { get; set; }
        public virtual DbSet<EDUCATION_DEGREE_LEVELS> EDUCATION_DEGREE_LEVELS { get; set; }
        public virtual DbSet<EXPERIENCE_TYPES> EXPERIENCE_TYPES { get; set; }
        public virtual DbSet<LOCATION> LOCATIONS { get; set; }
        public virtual DbSet<PERSON_DETAILS> PERSON_DETAILS { get; set; }
        public virtual DbSet<REFERENCE_TYPES> REFERENCE_TYPES { get; set; }
        public virtual DbSet<TERRITORy> TERRITORIES { get; set; }
        public virtual DbSet<USER_TYPES> USER_TYPES { get; set; }
        public virtual DbSet<FormPDEmplyeeView> FormPDEmplyeeViews { get; set; }
        public virtual DbSet<View_EmployeeListForAssignToParent> View_EmployeeListForAssignToParent { get; set; }
        public virtual DbSet<View_EMPLOYEES> View_EMPLOYEES { get; set; }
        public virtual DbSet<View_EMPLOYEES_DETAIL> View_EMPLOYEES_DETAIL { get; set; }
        public virtual DbSet<View_PERSON> View_PERSON { get; set; }
        public virtual DbSet<View_Supervisors> View_Supervisors { get; set; }
        public virtual DbSet<View_USERS> View_USERS { get; set; }
        public virtual DbSet<viewDistributionHouseSpecificPosition> viewDistributionHouseSpecificPositions { get; set; }
        public virtual DbSet<viewHouseAndTypeSpecificUnassignedEmployee> viewHouseAndTypeSpecificUnassignedEmployees { get; set; }
        public virtual DbSet<viewHouseSpecificPositionAndEmployee> viewHouseSpecificPositionAndEmployees { get; set; }
        public virtual DbSet<viewSearchEmployee> viewSearchEmployees { get; set; }
    }
}