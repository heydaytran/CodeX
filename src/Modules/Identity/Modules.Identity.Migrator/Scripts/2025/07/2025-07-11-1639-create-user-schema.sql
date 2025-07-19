
CREATE TABLE IF NOT EXISTS "identity"."data_protection_keys" (
    "id" serial PRIMARY KEY,
    "friendly_name" text,
    "xml" text
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_roles" (
    "id" text NOT NULL,
    "name" character varying(256),
    "normalized_name" character varying(256),
    "concurrency_stamp" text,
    CONSTRAINT "pk__asp_net_roles" PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_users" (
    "id" text NOT NULL,
    "user_name" character varying(256),
    "normalized_user_name" character varying(256),
    "email" character varying(256),
    "normalized_email" character varying(256),
    "email_confirmed" boolean NOT NULL,
    "password_hash" text,
    "security_stamp" text,
    "concurrency_stamp" text,
    "phone_number" text,
    "phone_number_confirmed" boolean NOT NULL,
    "two_factor_enabled" boolean NOT NULL,
    "lockout_end" timestamp with time zone,
    "lockout_enabled" boolean NOT NULL,
    "access_failed_count" integer NOT NULL,
    CONSTRAINT "pk__asp_net_users" PRIMARY KEY ("id")
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_role_claims" (
    "id" serial NOT NULL,
    "role_id" text NOT NULL,
    "claim_type" text,
    "claim_value" text,
    CONSTRAINT "pk__asp_net_role_claims" PRIMARY KEY ("id"),
    CONSTRAINT "fk__asp_net_role_claims__asp_net_roles__role_id" FOREIGN KEY ("role_id") REFERENCES "identity"."asp_net_roles" ("id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_user_claims" (
    "id" serial NOT NULL,
    "user_id" text NOT NULL,
    "claim_type" text,
    "claim_value" text,
    CONSTRAINT "pk__asp_net_user_claims" PRIMARY KEY ("id"),
    CONSTRAINT "fk__asp_net_user_claims__asp_net_users__user_id" FOREIGN KEY ("user_id") REFERENCES "identity"."asp_net_users" ("id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_user_logins" (
    "login_provider" character varying(128) NOT NULL,
    "provider_key" character varying(128) NOT NULL,
    "provider_display_name" text,
    "user_id" text NOT NULL,
    CONSTRAINT "pk__asp_net_user_logins" PRIMARY KEY ("login_provider", "provider_key"),
    CONSTRAINT "fk__asp_net_user_logins__asp_net_users__user_id" FOREIGN KEY ("user_id") REFERENCES "identity"."asp_net_users" ("id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_user_roles" (
    "user_id" text NOT NULL,
    "role_id" text NOT NULL,
    CONSTRAINT "pk__asp_net_user_roles" PRIMARY KEY ("user_id", "role_id"),
    CONSTRAINT "fk__asp_net_user_roles__asp_net_roles__role_id" FOREIGN KEY ("role_id") REFERENCES "identity"."asp_net_roles" ("id") ON DELETE CASCADE,
    CONSTRAINT "fk__asp_net_user_roles__asp_net_users__user_id" FOREIGN KEY ("user_id") REFERENCES "identity"."asp_net_users" ("id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "identity"."asp_net_user_tokens" (
    "user_id" text NOT NULL,
    "login_provider" character varying(128) NOT NULL,
    "name" character varying(128) NOT NULL,
    "value" text,
    CONSTRAINT "pk__asp_net_user_tokens" PRIMARY KEY ("user_id", "login_provider", "name"),
    CONSTRAINT "fk__asp_net_user_tokens__asp_net_users__user_id" FOREIGN KEY ("user_id") REFERENCES "identity"."asp_net_users" ("id") ON DELETE CASCADE
);

CREATE INDEX "ix__asp_net_role_claims__role_id" ON "identity"."asp_net_role_claims" ("role_id");
CREATE INDEX "role_name_index" ON "identity"."asp_net_roles" ("normalized_name");
CREATE INDEX "ix__asp_net_user_claims__user_id" ON "identity"."asp_net_user_claims" ("user_id");
CREATE INDEX "ix__asp_net_user_logins__user_id" ON "identity"."asp_net_user_logins" ("user_id");
CREATE INDEX "ix__asp_net_user_roles__role_id" ON "identity"."asp_net_user_roles" ("role_id");
CREATE INDEX "email_index" ON "identity"."asp_net_users" ("normalized_email");
CREATE INDEX "user_name_index" ON "identity"."asp_net_users" ("normalized_user_name");