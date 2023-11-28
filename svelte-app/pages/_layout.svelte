<!-- routify:options preload="proximity" -->

<script>
    import { setContext, onMount } from 'svelte';
    import util from './utils';
    import api from '../services/api';

    import ErrorPopup from '../shared/ErrorPopup.svelte';
    import ApiDependent from '../shared/ApiDependent.svelte';

    var user = null;
    setContext('user', {
        get: () => user,
        set: newUser => user = newUser
    });
    
    onMount(async () => user = await api.get('users/me', 'Failed fetching user data'));

</script>



<div class="d-flex flex-column h-100">
    <nav class="top-row p-3 border-bottom navbar shadow">
        <div class="container-fluid d-flex gap-3">
            <a class="navbar-brand nav-link text-dark" href="/">MyWords</a>
            <div class="flex-grow-1"></div>
            <!-- <a class="nav-link text-dark" href="login">Login</a>
            <a class="nav-link text-dark" href="signup">Sign Up</a> -->
            <!-- <button on:click={goToAccount}>Account</button> -->

            <ApiDependent ready={user != null}>
                <!-- <div slot="loading"></div> -->
                <div class="dropdown">
                    <button class="btn btn-secondary" data-bs-toggle="dropdown" aria-expanded="false">
                        { (user?.givenName ?? "") } { (user?.familyName ?? "") }
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><button class="dropdown-item" on:click={() => util.navigate('/account')}>My Account</button></li>
                        <li><button class="dropdown-item" on:click={() => util.navigateBackend('/api/identity/logout')}>Logout</button></li>
                    </ul>
                </div>
            </ApiDependent>
        </div>
    </nav>
    <main class="flex-shrink-0 px-4 py-4 text-start">
        <slot />
    </main>
    <ErrorPopup />
    <!-- <div class="footer p-3">
    </div> -->
</div>