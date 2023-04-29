using Books.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTg3MjI0M0AzMjMxMmUzMTJlMzMzNVNqU2cwTkNvQjJOTGRkZTB6YVdreEF2T1kyVUdjTUZqS21YbHZBdVAyTVE9;Mgo+DSMBaFt+QHFqVkNrWU5BaV1CX2BZfVl0RGlddk4QCV5EYF5SRHJdSlxmS39Xf0VlXXk=;Mgo+DSMBMAY9C3t2VFhhQlJBfVpdWHxLflF1VWBTfV96cFxWACFaRnZdQV1nS3ZSc0diXXZeeHJT;Mgo+DSMBPh8sVXJ1S0d+X1RPc0BHQmFJfFBmRmlbflR0fUUmHVdTRHRcQllhQX9WdERnUH5ecnQ=;MTg3MjI0N0AzMjMxMmUzMTJlMzMzNWNwa2lwckZaeUZycFhMcjR2ZTh4SjRYQ0c0NXcwTEJ1OEZkWHlKZzk0N2s9;NRAiBiAaIQQuGjN/V0d+XU9Hc1RHQmZWfFN0RnNadVxxflFPcDwsT3RfQF5jTXxbd0BhWHtXcX1RRg==;ORg4AjUWIQA/Gnt2VFhhQlJBfVpdWHxLflF1VWBTfV96cFxWACFaRnZdQV1nS3ZSc0diXXZccXNV;MTg3MjI1MEAzMjMxMmUzMTJlMzMzNVc1QmNTL0lJQVdWcWNvQ1JtWlk1MlM5TDgrNWxwbnpEZVloMldEMlpweFE9;MTg3MjI1MUAzMjMxMmUzMTJlMzMzNU1oL1ZtS2RUU3lFbVlXZk9kbTZvVUt3di9aSjZyY29BWCtCY2pjSWxWNUk9;MTg3MjI1MkAzMjMxMmUzMTJlMzMzNUg2b2RGbUVpeXMxQUNRNzZ5N25uY0IzUlRrcERvOVErZXFpbXBuSVkwMWc9;MTg3MjI1M0AzMjMxMmUzMTJlMzMzNWFDU1lWL1RpNzE4UWRYemUzSkdHOFR1aFJMZUt4a001U2NwQitWU284Qnc9;MTg3MjI1NEAzMjMxMmUzMTJlMzMzNVNqU2cwTkNvQjJOTGRkZTB6YVdreEF2T1kyVUdjTUZqS21YbHZBdVAyTVE9");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
