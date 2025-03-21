import dash
from dash import dcc, html, Input, Output
import plotly.express as px
import pandas as pd

# Sample data (Replace with your CSV data)
df = pd.read_csv("similarity_result.csv")
df.rename(columns={df.columns[0]: "Query"}, inplace=True)
dfq = pd.read_csv("query_embeddings.csv")
dfr = pd.read_csv("reference_embeddings.csv")

app = dash.Dash(__name__)

app.layout = html.Div([
    html.H1("Text Similarity Analysis", style={
        "text-align": "center",
        "font-family": "Arial, sans-serif",
        "color": "#333",
        "margin-bottom": "20px"
    }),

    html.Div([
    dcc.Dropdown(
        id="ref-selector",
        options=[{"label": ref, "value": ref} for ref in df.columns[1:]],  # Exclude first column (Query)
        multi=True,
        placeholder="Select References...",
        style={
        "font-family": "Arial, sans-serif",  # Change the font
        "font-size": "16px",  # Adjust font size
        "color": "#333",  # Text color
        # "background-color": "#f8f9fa",  # Background color
        "border-radius": "8px",  # Rounded corners
        "padding": "10px",
        # "box-shadow": "0 2px 5px rgba(0,0,0,0.2)",  # Light shadow effect
        "width": "60%",  # Adjust width for better alignment
        "margin": "auto"  # Centering the dropdown
    })], style={"display": "flex", "justify-content": "center","margin-bottom": "20px"}) ,

    dcc.Graph(id="similarity-graph"),

    html.H1("Scalar value Mapping of Embedding Vectors", style={
        "text-align": "center",
        "font-family": "Arial, sans-serif",
        "color": "#333",
        "margin-bottom": "20px"
    }),
    
    html.Div([
    dcc.Dropdown(
        id="query-selector",
        options=[{"label": ref, "value": ref} for ref in dfq["Categories"]],  
        multi=False,
        placeholder="Select query...",
        style={
        "font-family": "Arial, sans-serif",  # Change the font
        "font-size": "16px",  # Adjust font size
        "color": "#333",  # Text color
        # "background-color": "#f8f9fa",  # Background color
        "border-radius": "8px",  # Rounded corners
        "padding": "10px",
        # "box-shadow": "0 2px 5px rgba(0,0,0,0.2)",  # Light shadow effect
        "width": "60%",  # Adjust width for better alignment
        "margin": "auto"  # Centering the dropdown
    }),
    dcc.Dropdown(
        id="reference-selector",
        options=[{"label": ref, "value": ref} for ref in dfr["Categories"]],  
        multi=False,
        placeholder="Select reference...",
        style={
        "font-family": "Arial, sans-serif",  # Change the font
        "font-size": "16px",  # Adjust font size
        "color": "#333",  # Text color
        # "background-color": "#f8f9fa",  # Background color
        "border-radius": "8px",  # Rounded corners
        "padding": "10px",
        # "box-shadow": "0 2px 5px rgba(0,0,0,0.2)",  # Light shadow effect
        "width": "60%",  # Adjust width for better alignment
        "margin": "auto"  # Centering the dropdown
    })
    ], style={"display": "flex", "justify-content": "center","margin-bottom": "20px"}) ,

    dcc.Graph(id="embedding-graph")
])

@app.callback(
    Output("similarity-graph", "figure"),
    Input("ref-selector", "value")
)
def update_graph(selected_refs):
    if not selected_refs:
        return px.scatter(title="Select at least one reference")

    # Filter data for selected references
    filtered_df = df[["Query"] + selected_refs]
    hover_data = ["Query"] + selected_refs  # Ensures Category appears first

    if len(selected_refs) == 1:
        fig = px.bar(filtered_df, x="Query", y=selected_refs[0], title="Comparison of Similarity Scores for 1 Reference",color=selected_refs[0], color_continuous_scale="Blues",)
        fig.update_layout(yaxis=dict(range=[0, 1]))
        return fig
    elif len(selected_refs) > 3:
        melted_df = filtered_df.melt(id_vars=["Query"], var_name="Reference", value_name="Similarity")
        fig = px.bar(melted_df, x="Query", y="Similarity", color="Reference", title="Comparison of Similarity Scores for 3+ References", barmode="group", color_discrete_sequence=px.colors.qualitative.Set1 )
        fig.update_layout(yaxis=dict(range=[0, 1]))
        return fig
    elif len(selected_refs) == 2:
        fig = px.scatter(filtered_df, x=selected_refs[0], y=selected_refs[1], title="Comparison of Similarity Scores for 2 References", hover_data=hover_data, color="Query")
    elif len(selected_refs) == 3:
        fig = px.scatter_3d(filtered_df, x=selected_refs[0], y=selected_refs[1], z=selected_refs[2], title="Comparison of Similarity Scores for 3 References", hover_data=hover_data, color="Query")
    
    fig.update_layout(yaxis=dict(range=[0, 1]))
    fig.update_traces(marker=dict(size=12, opacity=0.7))  # Set all dots to size 10

    return fig

@app.callback(
    Output("embedding-graph", "figure"),
    [Input("query-selector", "value"), Input("reference-selector", "value")]
)
def update_embedding_graph(selected_query, selected_reference):
    if not selected_reference and not selected_query:
        return px.scatter(title="Select at least one query or reference")

    if(selected_reference and not selected_query):
        # Extract reference vector
        reference_vector = dfr[dfr["Categories"] == selected_reference].iloc[:, 1:].values.flatten()
        x_axis = list(range(len(reference_vector)))  # Embedding dimensions as X-axis   
  
        # Only reference selected
        fig = px.line(x=x_axis, y=reference_vector, 
                      title=f"Embedding Vector for {selected_reference}",
                      labels={"x": "Dimension", "y": "Embedding Value"},
                      markers=True)
        fig.update_layout(yaxis=dict(range=[-1, 1]))        
        fig.update_traces(line=dict(color='blue', width=2), marker=dict(size=8))
        return fig

    if(selected_query and not selected_reference):
        # Extract query vector
        query_vector = dfq[dfq["Categories"] == selected_query].iloc[:, 1:].values.flatten()
        x_axis = list(range(len(query_vector)))  # Embedding dimensions as X-axis   
        # Only query selected 
        fig = px.line(x=x_axis, y=query_vector, 
                      title=f"Embedding Vector for {selected_query}",
                      labels={"x": "Dimension", "y": "Embedding Value"},
                      markers=True)
        fig.update_layout(yaxis=dict(range=[-1, 1]))      
        fig.update_traces(line=dict(color='red', width=2), marker=dict(size=8))
        return fig    

    # Find similarity from similarity_result.csv
    similarity_score = df[df["Query"] == selected_query][selected_reference].values[0]

    # Plot both query & reference embeddings
    reference_vector = dfr[dfr["Categories"] == selected_reference].iloc[:, 1:].values.flatten()
    query_vector = dfq[dfq["Categories"] == selected_query].iloc[:, 1:].values.flatten()
    x_axis = list(range(len(query_vector)))  # Embedding dimensions as X-axis   

    fig = px.line(title=f"Embedding Comparison: {selected_query} vs {selected_reference}",
                  labels={"x": "Dimension", "y": "Embedding Value"})

    fig.add_scatter(x=x_axis, y=query_vector, mode="lines+markers", name=f"Query: {selected_query}",
                    line=dict(color='red', width=2), marker=dict(size=8))
    fig.add_scatter(x=x_axis, y=reference_vector, mode="lines+markers", name=f"Reference: {selected_reference}",
                    line=dict(color='blue', width=2), marker=dict(size=8))

    # Add similarity annotation
    fig.add_annotation(
        x=0.5, y=max(max(query_vector), max(reference_vector)), 
        text=f"Similarity: {similarity_score:.4f}",
        showarrow=False, font=dict(size=14, color="black"),
        xref="paper", yref="paper"
    )
    
    fig.update_layout(yaxis=dict(range=[-1, 1]))        

    return fig

if __name__ == "__main__":
    app.run_server(debug=True) 