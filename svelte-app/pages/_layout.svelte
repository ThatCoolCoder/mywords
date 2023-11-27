<!-- routify:options preload="proximity" -->

<script>
    import { setContext, onMount } from 'svelte';
    import util from './utils';
    import ErrorPopup from '../shared/errorPopup.svelte';

    var user = null;
    setContext('user', {
        get: () => user,
        set: newUser => user = newUser
    });

    async function fetchAccountInfo() {
        const response = await fetch('/api/users/me', {
            credentials: 'include'
        });
        user = await response.json();
        
    }
    
    onMount(fetchAccountInfo);

</script>



<div class="d-flex flex-column h-100">
    <nav class="top-row p-3 border-bottom navbar shadow">
        <div class="container-fluid d-flex gap-3">
            <a class="navbar-brand nav-link text-dark" href="/">MyWords</a>
            <div class="flex-grow-1"></div>
            <!-- <a class="nav-link text-dark" href="login">Login</a>
            <a class="nav-link text-dark" href="signup">Sign Up</a> -->
            <!-- <button on:click={goToAccount}>Account</button> -->

            <div class="dropdown">
                <button class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  { (user?.givenName ?? "") } { (user?.familyName ?? "") }
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li><button class="dropdown-item" on:click={() => util.navigate('/account')}>My Account</button></li>
                  <li><button class="dropdown-item" on:click={() => util.navigateBackend('/api/identity/logout')}>Logout</button></li>
                </ul>
              </div>
        </div>
    </nav>
    <main class="flex-shrink-0 px-4 py-4">
        <slot />
    </main>
    <ErrorPopup />
    <!-- <div class="footer p-3">
    </div> -->
</div>