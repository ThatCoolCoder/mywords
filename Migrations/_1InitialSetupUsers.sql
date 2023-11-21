begin transaction;

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

create table aspnetuser (
    id text primary key not null,
    username character varying(256),
    normalizedusername character varying(256) unique,
    email character varying(256),
    normalizedemail character varying(256),
    emailconfirmed boolean not null,
    passwordhash text,
    securitystamp text,
    concurrencystamp text,
    phonenumber text,
    phonenumberconfirmed boolean not null,
    twofactorenabled boolean not null,
    lockoutend timestamp with time zone,
    lockoutenabled boolean not null,
    accessfailedcount integer not null
);
create index ix_aspnetuser_email on aspnetuser using btree (normalizedemail);


create table aspnetrole (
    id text primary key not null,
    name character varying(256),
    normalizedname character varying(256) unique,
    concurrencystamp text
);

create table aspnetuserrole (
    userid text not null,
    roleid text not null,

    constraint pk_aspnetuserrole primary key (userid, roleid),

    constraint fk_user foreign key (userid) references aspnetuser(id) on delete cascade,
    constraint fk_role foreign key (roleid) references aspnetrole(id) on delete cascade
);
create index ix_aspnetuserrole_roleid on aspnetuserrole using btree (roleid);

create table aspnetroleclaim (
    id serial primary key not null,
    roleid text not null,
    claimtype text,
    claimvalue text,

    constraint fk_role foreign key (roleid) references aspnetrole(id) on delete cascade
);
create index ix_aspnetroleclaim_roleid on aspnetroleclaim using btree (roleid);


create table aspnetuserclaim (
    id serial primary key not null,
    userid text not null,
    claimtype text,
    claimvalue text,

    constraint fk_user foreign key (userid) references aspnetuser(id) on delete cascade
);
create index ix_aspnetuserclaim_userid on aspnetuserclaim using btree (userid);

create table aspnetuserlogin (
    loginprovider text not null,
    providerkey text not null,
    providerdisplayname text,
    userid text not null,

    constraint pk_aspnetuserlogin primary key (loginprovider, providerkey),

    constraint fk_user foreign key (userid) references aspnetuser(id) on delete cascade
);
create index ix_aspnetuserlogin_userid on aspnetuserlogin using btree (userid);


create table aspnetusertoken (
    userid text not null,
    loginprovider text not null,
    name text not null,
    value text,

    constraint pk_aspnetusertoken primary key (userid, loginprovider, name),

    constraint fk_user foreign key (userid) references aspnetuser(id) on delete cascade
);

commit;