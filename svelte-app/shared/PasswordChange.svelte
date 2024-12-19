<script>
    import { getContext } from "svelte";
    import { fade } from "svelte/transition";
    import api from "services/api";

    import Loader from "shared/misc/Loader.svelte";
    import errorService from "services/errorService";
    import A from "pages/[...fallback].svelte";

    const {close} = getContext('simple-modal');

    const Step = {
        PasswordInput: 0,
        AwaitingToken: 1,
        NewPasswordInput: 2,
        AwaitingSuccess: 3,
        Success: 4,
        Fail: 5,
    }

    let step = Step.PasswordInput;
    let password1 = "";
    let password2 = "";
    let token;
    let error = "";

    async function getToken() {
        error = "";
        step = Step.AwaitingToken;

        try {
            let response = await api.postJson('users/me/password/generatechangetoken', {password: password1}, "Failed getting password reset token");
            token = response.token;

            step = Step.NewPasswordInput;
            password1 = "";
        }
        catch (e) {
            if (e.response.status == 401) {
                error = "Incorrect password";
                step = Step.PasswordInput;
                return;
            }
            errorService.display('Unexpected error fetching reset token', e.format());
        }
    }

    async function sendChange() {
        error = "";
        step = Step.AwaitingSuccess;

        await api.safe.post('users/me/password/submitchange', {password: password1, confirmPassword: password2, token}, "Failed applying change");

        step = Step.Success;
    }
</script>

<div style="height: min(250px, 50vh)" class="d-flex flex-column">
    <div class="row mb-3">
        <h4>Change password</h4>
    </div>
    
    {#if step == Step.PasswordInput || step == Step.AwaitingToken}
        <div class="d-flex">
            <p>To continue, enter your current password</p>
            <input class="form-control" type="password" bind:value={password1}/>
        </div>
        <div class="d-flex">
            <a href="/RequestResetPassword" on:click={close} on:keypress={e => e.key == 'Enter' && close()}>Forgot password</a>
            <p class="text-danger ms-3">{error}</p>
        </div>
    
        {#if password1 != ""}
            <div class="d-flex justify-content-end mt-auto" in:fade>
                {#if step == Step.AwaitingToken}
                    <Loader />
                {/if}
                <button class="btn text-secondary" on:click={getToken}>Next<i class="bi-chevron-right ms-2"></i></button>
            </div>
        {/if}
    {:else if step == Step.NewPasswordInput || step == Step.AwaitingSuccess}
        
        <div class="mb-3 form-floating">
            <input class="form-control" type="password" bind:value={password1} id="pwd1"/>
            <label for="pwd1">Enter new password</label>
        </div>

        <div class="form-floating">
            <input class="form-control" type="password" bind:value={password2} id="pwd2"/>
            <label for="pwd2">Enter new password again</label>
        </div>

        {#if password1 != password2}
            <p class="text-danger" in:fade><i class="bi-exclamation-triangle me-2"></i>Passwords do not match</p>
        {/if}
    
        {#if password1 != "" && password2 != "" && password1 == password2}
            <div class="d-flex justify-content-end mt-auto" in:fade>
                {#if step == Step.AwaitingSuccess}
                    <Loader />
                {/if}
                <button class="btn text-secondary" on:click={sendChange}>Next<i class="bi-chevron-right ms-2"></i></button>
            </div>
        {/if}
    {:else if step == Step.Success}
        <div>
            Password updated successfully
        </div>
        <div class="d-flex mt-3">
            <button class="btn btn-secondary" on:click={close}>Ok</button>
        </div>
    {/if}
</div>