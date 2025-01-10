begin transaction;

create table Friendship (
    Id bigserial primary key,
    ApplicationUser1Id text not null,
    ApplicationUser2Id text not null,
    Accepted boolean not null,
    
    constraint fk_user1 foreign key (ApplicationUser1Id) references aspnetuser(Id) on delete cascade,
    constraint fk_user2 foreign key (ApplicationUser2Id) references aspnetuser(Id) on delete cascade
);

create table FriendshipLink (
    Id uuid primary key,
    SenderId text not null,
    SingleUse boolean not null,
    
    constraint fk_sender foreign key (SenderId) references aspnetuser(Id) on delete cascade
);

commit;