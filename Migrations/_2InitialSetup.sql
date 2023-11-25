-- Setup terms and stuff

begin transaction;

create table TermSet (
    Id bigserial primary key,
    Name text not null,
    Description text not null,
    ApplicationUserId text not null,
    
    constraint fk_user foreign key (ApplicationUserId) references aspnetuser(Id) on delete cascade
);

create table Term (
    Id bigserial primary key,
    TermSetId bigint not null,

    Value text not null,
    Definition text not null,
    Notes text not null,
    CurrentStreak int not null,
    MovedToCurrentListUtc timestamp not null,

    constraint fk_TermSet foreign key (TermSetId) references TermSet(Id) on delete cascade
);

create table Label (
    Id bigserial primary key,
    Name text not null,
    Color text not null,
    TermSetId bigint not null,

    constraint fk_TermSet foreign key (TermSetId) references TermSet(Id) on delete cascade
);

create table TermLabel (
    Id bigserial primary key,
    TermId bigint not null,
    LabelId bigint not null,

    constraint fk_term foreign key (TermId) references term(Id) on delete cascade,
    constraint fk_label foreign key (LabelId) references label(Id) on delete cascade
);

commit;