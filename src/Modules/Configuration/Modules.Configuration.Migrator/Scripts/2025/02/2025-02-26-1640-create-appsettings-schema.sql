CREATE TABLE IF NOT EXISTS configurations.app_settings (
   id UUID PRIMARY KEY,
   created_at TIMESTAMPTZ NOT NULL,
   last_activity_at TIMESTAMPTZ NOT NULL,
   app TEXT NOT NULL,
   name TEXT NOT NULL,
   value TEXT NULL,
   note TEXT NULL,
   target TEXT NULL,
   is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);
