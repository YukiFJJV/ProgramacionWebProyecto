using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsyLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase10InfrastructureIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "active_ingredients",
                columns: table => new
                {
                    ingredient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_active_ingredients", x => x.ingredient_id);
                });

            migrationBuilder.CreateTable(
                name: "diagnosis_catalog",
                columns: table => new
                {
                    diagnosis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diagnosis_catalog", x => x.diagnosis_id);
                });

            migrationBuilder.CreateTable(
                name: "diagnosis_statuses",
                columns: table => new
                {
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diagnosis_statuses", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    method = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.method_id);
                });

            migrationBuilder.CreateTable(
                name: "phone_numbers",
                columns: table => new
                {
                    phone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_numbers", x => x.phone_id);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.plan_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "specialties",
                columns: table => new
                {
                    specialty_id = table.Column<Guid>(type: "uuid", nullable: false),
                    specialty_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_specialties", x => x.specialty_id);
                });

            migrationBuilder.CreateTable(
                name: "laboratories",
                columns: table => new
                {
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laboratories", x => x.laboratory_id);
                    table.ForeignKey(
                        name: "FK_laboratories_phone_numbers_phone_id",
                        column: x => x.phone_id,
                        principalTable: "phone_numbers",
                        principalColumn: "phone_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    rtn = table.Column<string>(type: "text", nullable: true),
                    link_token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.tenant_id);
                    table.ForeignKey(
                        name: "FK_tenants_phone_numbers_phone_id",
                        column: x => x.phone_id,
                        principalTable: "phone_numbers",
                        principalColumn: "phone_id");
                });

            migrationBuilder.CreateTable(
                name: "subplans",
                columns: table => new
                {
                    subplan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cant_hours = table.Column<short>(type: "smallint", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subplans", x => x.subplan_id);
                    table.ForeignKey(
                        name: "FK_subplans_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "plan_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medications",
                columns: table => new
                {
                    medication_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<int>(type: "integer", nullable: false),
                    presentation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medications", x => x.medication_id);
                    table.CheckConstraint("ck_medications_content", "content > 0");
                    table.ForeignKey(
                        name: "FK_medications_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "laboratory_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    phone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_phone_numbers_phone_id",
                        column: x => x.phone_id,
                        principalTable: "phone_numbers",
                        principalColumn: "phone_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "diagnosis_medications",
                columns: table => new
                {
                    diagnosis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medication_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diagnosis_medications", x => new { x.diagnosis_id, x.medication_id });
                    table.ForeignKey(
                        name: "FK_diagnosis_medications_diagnosis_catalog_diagnosis_id",
                        column: x => x.diagnosis_id,
                        principalTable: "diagnosis_catalog",
                        principalColumn: "diagnosis_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_diagnosis_medications_medications_medication_id",
                        column: x => x.medication_id,
                        principalTable: "medications",
                        principalColumn: "medication_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medication_ingredients",
                columns: table => new
                {
                    medication_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingredient_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medication_ingredients", x => new { x.medication_id, x.ingredient_id });
                    table.ForeignKey(
                        name: "FK_medication_ingredients_active_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "active_ingredients",
                        principalColumn: "ingredient_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_medication_ingredients_medications_medication_id",
                        column: x => x.medication_id,
                        principalTable: "medications",
                        principalColumn: "medication_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.patient_id);
                    table.ForeignKey(
                        name: "FK_patients_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patients_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "psychologists",
                columns: table => new
                {
                    psychologist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    specialty_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    license_number = table.Column<string>(type: "char(6)", nullable: false),
                    biography = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_psychologists", x => x.psychologist_id);
                    table.ForeignKey(
                        name: "FK_psychologists_specialties_specialty_id",
                        column: x => x.specialty_id,
                        principalTable: "specialties",
                        principalColumn: "specialty_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_psychologists_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_psychologists_users_admin_id",
                        column: x => x.admin_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_psychologists_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subplan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.subscription_id);
                    table.ForeignKey(
                        name: "FK_subscriptions_subplans_subplan_id",
                        column: x => x.subplan_id,
                        principalTable: "subplans",
                        principalColumn: "subplan_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscriptions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fiscal_authorizations",
                columns: table => new
                {
                    fiscal_authorization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cai = table.Column<string>(type: "char(32)", nullable: false),
                    limit_date = table.Column<DateOnly>(type: "date", nullable: false),
                    initial_range = table.Column<string>(type: "char(16)", nullable: false),
                    final_range = table.Column<string>(type: "char(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fiscal_authorizations", x => x.fiscal_authorization_id);
                    table.ForeignKey(
                        name: "FK_fiscal_authorizations_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fiscal_authorizations_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_diagnoses",
                columns: table => new
                {
                    diagnosis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_diagnoses", x => new { x.diagnosis_id, x.patient_id });
                    table.ForeignKey(
                        name: "FK_patient_diagnoses_diagnosis_catalog_diagnosis_id",
                        column: x => x.diagnosis_id,
                        principalTable: "diagnosis_catalog",
                        principalColumn: "diagnosis_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_diagnoses_diagnosis_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "diagnosis_statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_diagnoses_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    psychologist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_at = table.Column<DateOnly>(type: "date", nullable: false),
                    starts_at = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ends_at = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    reschedule_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.appointment_id);
                    table.CheckConstraint("ck_appointments_reschedule_count", "reschedule_count >= 0");
                    table.CheckConstraint("ck_appointments_valid_time", "ends_at > starts_at");
                    table.ForeignKey(
                        name: "FK_appointments_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_psychologists_psychologist_id",
                        column: x => x.psychologist_id,
                        principalTable: "psychologists",
                        principalColumn: "psychologist_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_schedules",
                columns: table => new
                {
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    psychologist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    week_day = table.Column<int>(type: "integer", nullable: false),
                    starts_at = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ends_at = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_schedules", x => x.schedule_id);
                    table.CheckConstraint("ck_work_schedules_valid_time", "ends_at > starts_at");
                    table.ForeignKey(
                        name: "FK_work_schedules_psychologists_psychologist_id",
                        column: x => x.psychologist_id,
                        principalTable: "psychologists",
                        principalColumn: "psychologist_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_schedules_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment_history",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_authorization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    issue_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    bill_number = table.Column<string>(type: "char(16)", nullable: true),
                    discount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_history", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_payment_history_fiscal_authorizations_fiscal_authorization_~",
                        column: x => x.fiscal_authorization_id,
                        principalTable: "fiscal_authorizations",
                        principalColumn: "fiscal_authorization_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_payment_history_payment_methods_method_id",
                        column: x => x.method_id,
                        principalTable: "payment_methods",
                        principalColumn: "method_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "patient_records",
                columns: table => new
                {
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    risk_score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_records", x => x.record_id);
                    table.CheckConstraint("ck_patient_records_risk_score", "risk_score >= 0 AND risk_score <= 3");
                    table.ForeignKey(
                        name: "FK_patient_records_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "appointment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_records_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patient_records_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bill_details",
                columns: table => new
                {
                    detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    detail = table.Column<string>(type: "text", nullable: false),
                    cant_hours = table.Column<short>(type: "smallint", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bill_details", x => x.detail_id);
                    table.ForeignKey(
                        name: "FK_bill_details_payment_history_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payment_history",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bill_details_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "tenant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prescriptions",
                columns: table => new
                {
                    prescription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medication_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dosage = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescriptions", x => x.prescription_id);
                    table.CheckConstraint("ck_prescriptions_quantity", "quantity IS NULL OR quantity > 0");
                    table.ForeignKey(
                        name: "FK_prescriptions_medications_medication_id",
                        column: x => x.medication_id,
                        principalTable: "medications",
                        principalColumn: "medication_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prescriptions_patient_records_record_id",
                        column: x => x.record_id,
                        principalTable: "patient_records",
                        principalColumn: "record_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "record_diagnoses",
                columns: table => new
                {
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    diagnosis_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_record_diagnoses", x => new { x.record_id, x.diagnosis_id });
                    table.ForeignKey(
                        name: "FK_record_diagnoses_diagnosis_catalog_diagnosis_id",
                        column: x => x.diagnosis_id,
                        principalTable: "diagnosis_catalog",
                        principalColumn: "diagnosis_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_record_diagnoses_patient_records_record_id",
                        column: x => x.record_id,
                        principalTable: "patient_records",
                        principalColumn: "record_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_patient_id_date_at",
                table: "appointments",
                columns: new[] { "patient_id", "date_at" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_psychologist_id_date_at_starts_at",
                table: "appointments",
                columns: new[] { "psychologist_id", "date_at", "starts_at" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_tenant_id",
                table: "appointments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_bill_details_payment_id",
                table: "bill_details",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_bill_details_tenant_id",
                table: "bill_details",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_diagnosis_medications_medication_id",
                table: "diagnosis_medications",
                column: "medication_id");

            migrationBuilder.CreateIndex(
                name: "IX_fiscal_authorizations_cai",
                table: "fiscal_authorizations",
                column: "cai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fiscal_authorizations_patient_id",
                table: "fiscal_authorizations",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_fiscal_authorizations_tenant_id",
                table: "fiscal_authorizations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_laboratories_phone_id",
                table: "laboratories",
                column: "phone_id");

            migrationBuilder.CreateIndex(
                name: "IX_medication_ingredients_ingredient_id",
                table: "medication_ingredients",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_medications_laboratory_id",
                table: "medications",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_patient_id",
                table: "patient_diagnoses",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_diagnoses_status_id",
                table: "patient_diagnoses",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_records_appointment_id",
                table: "patient_records",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_patient_records_patient_id",
                table: "patient_records",
                column: "patient_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patient_records_tenant_id",
                table: "patient_records",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_tenant_id",
                table: "patients",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_user_id",
                table: "patients",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_history_fiscal_authorization_id",
                table: "payment_history",
                column: "fiscal_authorization_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_history_issue_date",
                table: "payment_history",
                column: "issue_date");

            migrationBuilder.CreateIndex(
                name: "IX_payment_history_method_id",
                table: "payment_history",
                column: "method_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_numbers_number",
                table: "phone_numbers",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prescriptions_medication_id",
                table: "prescriptions",
                column: "medication_id");

            migrationBuilder.CreateIndex(
                name: "IX_prescriptions_record_id",
                table: "prescriptions",
                column: "record_id");

            migrationBuilder.CreateIndex(
                name: "IX_psychologists_admin_id",
                table: "psychologists",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_psychologists_license_number",
                table: "psychologists",
                column: "license_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_psychologists_specialty_id",
                table: "psychologists",
                column: "specialty_id");

            migrationBuilder.CreateIndex(
                name: "IX_psychologists_tenant_id",
                table: "psychologists",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_psychologists_user_id",
                table: "psychologists",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_record_diagnoses_diagnosis_id",
                table: "record_diagnoses",
                column: "diagnosis_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_specialties_specialty_name",
                table: "specialties",
                column: "specialty_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subplans_plan_id",
                table: "subplans",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_subplan_id",
                table: "subscriptions",
                column: "subplan_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_tenant_id",
                table: "subscriptions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_name",
                table: "tenants",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_phone_id",
                table: "tenants",
                column: "phone_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_rtn",
                table: "tenants",
                column: "rtn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_phone_id",
                table: "users",
                column: "phone_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_tenant_id_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_schedules_psychologist_id_week_day_starts_at_ends_at",
                table: "work_schedules",
                columns: new[] { "psychologist_id", "week_day", "starts_at", "ends_at" });

            migrationBuilder.CreateIndex(
                name: "IX_work_schedules_tenant_id",
                table: "work_schedules",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bill_details");

            migrationBuilder.DropTable(
                name: "diagnosis_medications");

            migrationBuilder.DropTable(
                name: "medication_ingredients");

            migrationBuilder.DropTable(
                name: "patient_diagnoses");

            migrationBuilder.DropTable(
                name: "prescriptions");

            migrationBuilder.DropTable(
                name: "record_diagnoses");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "work_schedules");

            migrationBuilder.DropTable(
                name: "payment_history");

            migrationBuilder.DropTable(
                name: "active_ingredients");

            migrationBuilder.DropTable(
                name: "diagnosis_statuses");

            migrationBuilder.DropTable(
                name: "medications");

            migrationBuilder.DropTable(
                name: "diagnosis_catalog");

            migrationBuilder.DropTable(
                name: "patient_records");

            migrationBuilder.DropTable(
                name: "subplans");

            migrationBuilder.DropTable(
                name: "fiscal_authorizations");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "laboratories");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "psychologists");

            migrationBuilder.DropTable(
                name: "specialties");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "phone_numbers");
        }
    }
}
