<script>
    import api from 'services/api';
    import { notBlank } from 'shared/misc/validation';
    import  { navigate } from "pages/utils";
    import errorService from 'services/errorService';

    async function create(e) {
        e.preventDefault();
        let response = await api.post('collections/', {name, description}, 'Failed creating new collection');
        let ok = false;
        let error;

        let displayError =  (e) => {
            errorService.display("Failed navigating to newly created collection, please go to the homepage", e);
        }

        try {
            let json = await response.json();
            if (! Object.hasOwn(json, "id")) displayError("Expected api to return created collection id but it did not");
            navigate(`/collections/${json.id}`);
        }
        catch (e) {
            displayError(e);
        }
    }

    let name = "";
    let description = "";

</script>

<title>Create Collection | MyWords</title>

<form on:submit={create}>
    <div class="w-100 d-flex flex-column gap-3 align-items-center">
        <h2>Let's create a new collection</h2>

        <label class="thingo">
            <input
                class="form-control"
                autocomplete="off"
                placeholder="Choose a name..."
                bind:value={name}
                on:input={notBlank}
                required
            />
        </label>
        <label class="thingo">
            <textarea class="form-control" bind:value={description} placeholder="Optionally, add a more detailed description"></textarea>
        </label>
        <input type="submit" class="btn btn-primary" value="Done!" />
    </div>
</form>

<style>
    .thingo {
        min-width: 50ch;
    }
</style>