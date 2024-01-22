begin transaction;

alter table termset rename to Collection;

alter sequence termset_id_seq rename to collection_id_seq;

alter table Collection
    rename constraint termset_pkey to collection_pkey;

alter table Term
    rename constraint fk_TermSet to fk_Collection;

alter table Term
    rename column termsetid to CollectionId;

alter table Label
    rename constraint fk_TermSet to fk_Collection;

alter table Label
    rename column termsetid to CollectionId;

commit;