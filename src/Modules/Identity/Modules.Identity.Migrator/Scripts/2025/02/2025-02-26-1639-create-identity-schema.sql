CREATE SCHEMA IF NOT EXISTS identity;

CREATE TABLE IF NOT EXISTS identity.api_keys (
    id SERIAL PRIMARY KEY,
    api_key_hash TEXT NOT NULL UNIQUE,
    api_key_name TEXT NOT NULL,
    application_name TEXT NOT NULL CHECK (length(application_name) > 0),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expires_at TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    roles TEXT[]  -- Array of roles
    );


CREATE INDEX IF NOT EXISTS idx_api_key_hash ON identity.api_keys (api_key_hash);

CREATE TABLE IF NOT EXISTS identity.data_protection_keys (
    id SERIAL PRIMARY KEY,
    friendly_name TEXT NULL,
    xml TEXT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW()
    );