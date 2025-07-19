CREATE SCHEMA IF NOT EXISTS common;

 -- tenants table (Each tenant represents an organisation using the platform)
CREATE TABLE  IF NOT EXISTS common.tenants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique tenant ID
    code VARCHAR(100) UNIQUE , -- Unique tenant code
    name VARCHAR(100) , -- Display name of the tenant
    slug VARCHAR(100) , -- Used in URLs/domains (e.g. shearings)
    domain VARCHAR(255), -- Optional: custom domain
    branding JSONB, -- Logo, colours, etc. (optional)
    is_active BOOLEAN DEFAULT TRUE, -- Active status
    created_at TIMESTAMP  DEFAULT CURRENT_TIMESTAMP -- Creation date
);