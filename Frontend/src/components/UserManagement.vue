<template>
  <div>
    <v-toolbar flat color="white">
      <v-toolbar-title>User Management</v-toolbar-title>
      <v-divider
        class="mx-2"
        inset
        vertical
      ></v-divider>
      <v-spacer></v-spacer>
      <v-dialog v-model="dialog" max-width="500px">
        <template v-slot:activator="{ on }">
          <v-btn color="primary" dark class="mb-2" v-on="on">New User</v-btn>
        </template>
        <v-card>
          <v-card-title>
            <span class="headline">{{ formTitle }}</span>
          </v-card-title>

          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12 sm12 md12>
                  <v-text-field v-model="editedItem.id" label="Manager ID"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.city" label="City"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.state" label="State"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-text-field v-model="editedItem.country" label="Country"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6 md4>
                  <v-checkbox v-model="editedItem.disabled" label="Disabled"/>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" flat @click="close">Cancel</v-btn>
            <v-btn color="blue darken-1" flat @click="save">Save</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-toolbar>
    <v-data-table
      :headers="headers"
      :items="users"
      class="elevation-1"
    >
      <template v-slot:items="props">
        <td class="text-xs-left">{{ props.item.id }}</td>
        <td class="text-xs-left">{{ props.item.username }}</td>
        <td class="text-xs-left">{{ props.item.manager }}</td>
        <td class="text-xs-center">{{ props.item.disabled }}</td>
        <td class="text-xs-left">{{ props.item.city }}</td>
        <td class="text-xs-left">{{ props.item.state }}</td>
        <td class="text-xs-left">{{ props.item.country }}</td>
        <td class="justify-center layout px-0">
          <v-icon
            small
            class="mr-2"
            @click="editItem(props.item)"
          >
            edit
          </v-icon>
          <v-icon
            small
            @click="deleteItem(props.item)"
          >
            delete
          </v-icon>
        </td>
      </template>
      <template v-slot:no-data>
        <h2> {{ StatusOfData }} </h2>
        <v-btn color="primary" @click="initialize">Reset</v-btn>
      </template>
    </v-data-table>
  </div>
</template>

<script>
import { GetUsers, UpdateUser, DeleteUser } from '@/services/userManagementServices';
  export default {
      name: 'UserManagement',
      data: () => ({
        dialog: false,
        headers: [
          {
            text: 'ID',
            align: 'left',
            sortable: false,
            value: 'id'
          },
          { text: 'Username', value: 'username' },
          { text: 'Manager', value: 'manager' },
          { text: 'Disabled', value: 'disabled' },
          { text: 'City', value: 'city' },
          { text: 'State', value: 'state'},
          { text: 'Country', value: 'country'},
          { text: 'Actions', value: 'name', sortable: 'false'}
        ],
        users: [],
        editedIndex: -1,
        editedItem: {
          id: '',
          city: '',
          state: '',
          country: '',
          managerId: '',
          disabled: false
        },
        defaultItem: {
          userId: '',
          city: '',
          state: '',
          country: '',
          managerId: '',
          disabled: false
        },
        StatusOfData: 'No Data'
    }),

    computed: {
      formTitle () {
        return this.editedIndex === -1 ? 'New User' : 'Edit Item'
      }
    },

    watch: {
      dialog (val) {
        val || this.close()
      }
    },

    created () {
      this.initialize();
      console.log("Rendered");
    },

    methods: {
      initialize () {
        this.users = []
        this.StatusOfData = 'Loading Data...'
        GetUsers().then(
          response => {
            this.users = response;
          }
        )
        .catch(this.StatusOfData = 'No Data Found.')
      },

      editItem (item) {
        this.editedIndex = this.users.indexOf(item)
        this.editedItem = Object.assign({}, item)
        this.dialog = true
      },

      deleteItem (item) {
        const index = this.users.indexOf(item)
        if (confirm('Are you sure you want to delete this item?') && this.users.splice(index, 1)){
          DeleteUser(item.id)
            .then(console.log(response))
            .catch(err => {
              console.log(err.response.status);
            });
        }
      },

      close () {
        this.dialog = false
        setTimeout(() => {
          this.editedItem = Object.assign({}, this.defaultItem)
          this.editedIndex = -1
        }, 300)
      },

      save () {
        if (this.editedIndex > -1) {
          console.log(this.editedItem);
          UpdateUser(this.editedItem)
            .catch(console.log(response))
          Object.assign(this.users[this.editedIndex], this.editedItem)
        } else {
          this.desserts.push(this.editedItem)
        }
        this.close()
      }
    }
  }
</script>